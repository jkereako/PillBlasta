using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class MapGenerator: MonoBehaviour {
  public Map[] maps;
  public int mapIndex;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navMeshFloor;
  public Transform navMeshMaskPrefab;

  const string containerName = "GeneratedMap";
  Map currentMap;
  List<Coordinate> tileCoordinates;
  Queue<Coordinate> shuffledTileCoordinates;

  void Start() {
    GenerateMap();
  }

  public void GenerateMap() {
    currentMap = maps[mapIndex];

    Coordinate mapCenter = currentMap.center;
    Coordinate mapSize = currentMap.size;
    float tileSize = currentMap.tileSize;
    Vector2 maxMapSize = currentMap.maxSize;
    System.Random random = new System.Random(currentMap.seed);
    GetComponent<BoxCollider>().size = new Vector3(
      currentMap.size.x * tileSize, 0.5f, currentMap.size.y * tileSize
    );
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

    CreateTiles(currentMap, containerObject);

    bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
    int obstacleCount = (int)(mapSize.x * mapSize.y * currentMap.obstaclePercent);
    int currentObstacleCount = 0;
    // Generate the obstalces
    for (int i = 0; i < obstacleCount; i++) {
      Coordinate coordinate = GetRandomCoordinate();
      float obstacleHeight = 0.0f;
      Renderer obstacleRenderer;
      Material obstacleMaterial;
      float colorPercent = 0.0f;
      obstacleMap[coordinate.x, coordinate.y] = true;
      currentObstacleCount += 1;

      if (coordinate == mapCenter || !currentMap.IsMapCompletelyAccessible(obstacleMap, i)) {
        currentObstacleCount -= 1;
        obstacleMap[coordinate.x, coordinate.y] = false;
        continue;
      }

      obstacleHeight = Mathf.Lerp(
        currentMap.obstacleData.minHeight, 
        currentMap.obstacleData.maxHeight,
        (float)random.NextDouble()
      );
      Vector3 obstaclePosition = currentMap.CoordinateToPosition(coordinate);
      Transform obstacle = Instantiate(
                             obstaclePrefab, 
                             obstaclePosition + Vector3.up * obstacleHeight / 2.0f, 
                             Quaternion.identity);
      obstacle.localScale = new Vector3(
        (1 - currentMap.tileSeparatorWidth) * tileSize, obstacleHeight, (1 - currentMap.tileSeparatorWidth) * tileSize
      );
      colorPercent = (float)coordinate.y / (float)currentMap.size.y;
      obstacleRenderer = obstacle.GetComponent<Renderer>();
      obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
      obstacleRenderer.sharedMaterial = obstacleMaterial;
      obstacleMaterial.color = Color.Lerp(
        currentMap.obstacleData.foregroundColor, 
        currentMap.obstacleData.backgroundColor,
        colorPercent);
      obstacle.parent = containerObject;
    }

    CreateMapMask(currentMap, navMeshMaskPrefab, containerObject);

    navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
  }

  void CreateTiles(Map map, Transform containerObject) {
    for (int x = 0; x < map.size.x; x++) {
      for (int y = 0; y < map.size.y; y++) {
        Vector3 position;
        Transform tile;

        position = map.CoordinateToPosition(new Coordinate(x, y));
        tile = Instantiate(tilePrefab, position, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - map.tileSeparatorWidth) * map.tileSize;
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }
  }

  void CreateMapMask(Map map, Transform maskPrefab, Transform containerObject) {
    // The seemingly redundant code below masks the large nav mesh object so that the enemies cannot
    // walk outside of the map.
    // https://youtu.be/vQgLdFNrCN8?t=405
    Transform maskLeft, maskRight, maskTop, maskBottom;

    maskLeft = Instantiate(
      maskPrefab, 
      Vector3.left * (map.size.x + map.maxSize.x) / 4.0f * map.tileSize, 
      Quaternion.identity) as Transform;
    maskLeft.localScale = new Vector3(
      (map.size.x - map.maxSize.x) / 2.0f, 1, map.size.y) * map.tileSize;
    maskLeft.parent = containerObject;

    maskRight = Instantiate(
      maskPrefab, 
      Vector3.right * (map.size.x + map.maxSize.x) / 4.0f * map.tileSize, 
      Quaternion.identity) as Transform;
    maskRight.localScale = new Vector3(
      (map.size.x - map.maxSize.x) / 2.0f, 1, map.size.y) * map.tileSize;
    maskRight.parent = containerObject;

    maskTop = Instantiate(
      maskPrefab, 
      Vector3.forward * (map.size.x + map.maxSize.x) / 4.0f * map.tileSize, 
      Quaternion.identity) as Transform;
    maskTop.localScale = new Vector3(
      map.maxSize.x, 1, (map.maxSize.y - map.size.y) / 2.0f) * map.tileSize;
    maskTop.parent = containerObject;

    maskBottom = Instantiate(
      maskPrefab, 
      Vector3.back * (map.size.x + map.maxSize.x) / 4.0f * map.tileSize, 
      Quaternion.identity) as Transform;
    maskBottom.localScale = new Vector3(
      map.maxSize.x, 1, (map.maxSize.y - map.size.y) / 2.0f) * map.tileSize;
    maskBottom.parent = containerObject;
  }

  Coordinate GetRandomCoordinate() {
    // This cleaver trick ensures we never run out of coordinates.
    Coordinate coordinate = shuffledTileCoordinates.Dequeue();
    shuffledTileCoordinates.Enqueue(coordinate);

    return coordinate;
  }
}
