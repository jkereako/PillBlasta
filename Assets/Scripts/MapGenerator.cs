using UnityEngine;
using System.Collections.Generic;

public class MapGenerator: MonoBehaviour {
  public Map[] maps;
  public int mapIndex;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navMeshFloor;
  public Transform navMeshMaskPrefab;
  public Vector2 maxMapSize;
  public float tileSize;
  [Range(0, 1)]
  public float outLinePercent;

  Map currentMap;
  List<Coordinate> tileCoordinates;
  Queue<Coordinate> shuffledTileCoordinates;

  void Start() {
    GenerateMap();
  }

  public void GenerateMap() {
    currentMap = maps[mapIndex];
    const string containerName = "GeneratedMap";
    Coordinate mapCenter = currentMap.center;
    Coordinate mapSize = currentMap.size;
    System.Random random = new System.Random(currentMap.seed);

    tileCoordinates = new List<Coordinate>();
    for (int x = 0; x < mapSize.x; x++) {
      for (int y = 0; y < mapSize.y; y++) {
        tileCoordinates.Add(new Coordinate(x, y));
      }
    }

    shuffledTileCoordinates = new Queue<Coordinate>(Utility.Shuffle(tileCoordinates.ToArray(), currentMap.seed));

    // First, immediately destroy the container object if it already exists.
    if (transform.FindChild(containerName)) {
      DestroyImmediate(transform.FindChild(containerName).gameObject);
    }
      
    // Next, create a new game object at run-time to contain all of the newly generated tiles.
    Transform containerObject = new GameObject(containerName).transform;
    containerObject.parent = transform;

    // Generate the tiles
    for (int x = 0; x < mapSize.x; x++) {
      for (int y = 0; y < mapSize.y; y++) {
        Vector3 tilePosition = CoordinateToPosition(x, y);
        Transform tile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - outLinePercent) * tileSize;
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }

    bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
    int obstacleCount = (int)(mapSize.x * mapSize.y * currentMap.obstaclePercent);
    int currentObstacleCount = 0;
    // Generate the obstalces
    for (int i = 0; i < obstacleCount; i++) {
      Coordinate coordinate = GetRandomCoordinate();
      float obstacleHeight = 0.0f;
      obstacleMap[coordinate.x, coordinate.y] = true;
      currentObstacleCount += 1;

      if (coordinate == mapCenter || !currentMap.IsMapCompletelyAccessible(obstacleMap, i)) {
        currentObstacleCount -= 1;
        obstacleMap[coordinate.x, coordinate.y] = false;
        continue;
      }

      obstacleHeight = Mathf.Lerp(
        currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)random.NextDouble()
      );
      Vector3 obstaclePosition = CoordinateToPosition(coordinate.x, coordinate.y);
      Transform obstacle = Instantiate(
                             obstaclePrefab, 
                             obstaclePosition + Vector3.up * obstacleHeight / 2, 
                             Quaternion.identity);
      obstacle.localScale = new Vector3(
        (1 - outLinePercent) * tileSize, obstacleHeight, (1 - outLinePercent) * tileSize
      );
      obstacle.parent = containerObject;
    }

    // The seemingly redundant code below masks the large nav mesh object so that the enemies cannot
    // walk outside of the map.
    // https://youtu.be/vQgLdFNrCN8?t=405

    Transform navMeshMaskLeft = Instantiate(
                                  navMeshMaskPrefab, 
                                  Vector3.left * (mapSize.x + maxMapSize.x) / 4 * tileSize, 
                                  Quaternion.identity) as Transform;
    navMeshMaskLeft.localScale = new Vector3((mapSize.x - maxMapSize.x) / 2, 1, mapSize.y) * tileSize;
    navMeshMaskLeft.parent = containerObject;

    Transform navMeshMaskRight = Instantiate(
                                   navMeshMaskPrefab, 
                                   Vector3.right * (mapSize.x + maxMapSize.x) / 4 * tileSize, 
                                   Quaternion.identity) as Transform;
    navMeshMaskRight.localScale = new Vector3((mapSize.x - maxMapSize.x) / 2, 1, mapSize.y) * tileSize;
    navMeshMaskRight.parent = containerObject;

    Transform navMeshMaskTop = Instantiate(
                                 navMeshMaskPrefab, 
                                 Vector3.forward * (mapSize.x + maxMapSize.x) / 4 * tileSize, 
                                 Quaternion.identity) as Transform;
    navMeshMaskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;
    navMeshMaskTop.parent = containerObject;

    Transform navMeshMaskBottom = Instantiate(
                                    navMeshMaskPrefab, 
                                    Vector3.back * (mapSize.x + maxMapSize.x) / 4 * tileSize, 
                                    Quaternion.identity) as Transform;
    navMeshMaskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;
    navMeshMaskBottom.parent = containerObject;

    navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
  }

  Vector3 CoordinateToPosition(int x, int y) {
    return new Vector3(-currentMap.size.x / 2 + 0.5f + x, 0, -currentMap.size.y / 2 + 0.5f + y) * tileSize;
  }

  Coordinate GetRandomCoordinate() {
    // This cleaver trick ensures we never run out of coordinates.
    Coordinate coordinate = shuffledTileCoordinates.Dequeue();
    shuffledTileCoordinates.Enqueue(coordinate);

    return coordinate;
  }

}
