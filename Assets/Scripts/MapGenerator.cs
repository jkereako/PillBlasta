using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(BoxCollider))]
public class MapGenerator: MonoBehaviour {
  public Map[] maps;
  public int mapIndex;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navMeshFloor;
  public Transform navMeshMaskPrefab;

  const string containerName = "GeneratedMap";
  Queue<Coordinate> shuffledTileCoordinates;

  void Start() {
    GenerateMap();
  }

  public void GenerateMap() {
    // First, immediately destroy the container object if it already exists.
    if (transform.FindChild(containerName)) {
      DestroyImmediate(transform.FindChild(containerName).gameObject);
    }

    // Next, create a new game object at run-time to contain all of the newly generated tiles.
    Transform containerObject = new GameObject(containerName).transform;
    containerObject.parent = transform;
       
    Map map = maps[mapIndex];
    List<Coordinate> tileCoordinates = CreateTileCoordinates(map);
    shuffledTileCoordinates = new Queue<Coordinate>(
      Utility.Shuffle(tileCoordinates.ToArray(), map.seed)
    );

    CreateTiles(map, tilePrefab, containerObject);
    CreateObstacles(map, obstaclePrefab, containerObject);
    CreateMapMask(map, navMeshMaskPrefab, containerObject);

    GetComponent<BoxCollider>().size = new Vector3(
      map.size.x * map.tileSize, 0.5f, map.size.y * map.tileSize
    );

    navMeshFloor.localScale = new Vector3(map.maxSize.x, map.maxSize.y) * map.tileSize;
  }

  List<Coordinate> CreateTileCoordinates(Map map) {
    List<Coordinate> coordinates = new List<Coordinate>();

    for (int x = 0; x < map.size.x; x++) {
      for (int y = 0; y < map.size.y; y++) {
        coordinates.Add(new Coordinate(x, y));
      }
    }

    return coordinates;
  }

  void CreateTiles(Map map, Transform prefab, Transform containerObject) {
    for (int x = 0; x < map.size.x; x++) {
      for (int y = 0; y < map.size.y; y++) {
        Vector3 position;
        Transform tile;

        position = map.CoordinateToPosition(new Coordinate(x, y));
        tile = Instantiate(prefab, position, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - map.tileSeparatorWidth) * map.tileSize;

        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }
  }

  void CreateObstacles(Map map, Transform prefab, Transform containerObject) {
    System.Random random = new System.Random(map.seed);
    // Creates a 2D array of bools that is initialized to false.
    bool[,] obstacleMap = new bool[(int)map.size.x, (int)map.size.y];
    int limit = (int)(map.size.x * map.size.y * map.obstacleFill);
    int obstacleCount = 0;

    // Generate the obstalces
    for (int i = 0; i < limit; i++) {
      Coordinate coordinate = GetRandomCoordinate();
      obstacleMap[coordinate.x, coordinate.y] = true;
      obstacleCount += 1;

      if (coordinate == map.center || !map.IsMapCompletelyAccessible(obstacleMap, obstacleCount)) {
        obstacleCount -= 1;
        obstacleMap[coordinate.x, coordinate.y] = false;
        continue;
      }

      Vector3 position;
      Transform obstacle;
      float height = 0.0f;

      height = Mathf.Lerp(
        map.obstacleData.minHeight, map.obstacleData.maxHeight, (float)random.NextDouble()
      );
      position = map.CoordinateToPosition(coordinate);
      obstacle = Instantiate(
        prefab, position + Vector3.up * height / 2.0f, Quaternion.identity
      );

      ConfigureObstacle(map, coordinate, height, obstacle);
      
      obstacle.parent = containerObject;
    }
  }

  void ConfigureObstacle(Map map, Coordinate coordinate, float height, Transform obstacle) {
    Renderer theRenderer;
    Material material;
    float colorGradientMultiplier = 0.0f;

    obstacle.localScale = new Vector3(
      (1 - map.tileSeparatorWidth) * map.tileSize, 
      height, 
      (1 - map.tileSeparatorWidth) * map.tileSize
    );

    // `colorGradientMultiplier` is a tile's position on the map relative to the total size of the
    // map.
    colorGradientMultiplier = (float)coordinate.y / (float)map.size.y;
    theRenderer = obstacle.GetComponent<Renderer>();
    material = new Material(theRenderer.sharedMaterial);
    theRenderer.sharedMaterial = material;
    material.color = Color.Lerp(
      map.obstacleData.foregroundColor, map.obstacleData.backgroundColor, colorGradientMultiplier
    );
  }

  void CreateMapMask(Map map, Transform maskPrefab, Transform containerObject) {
    // The seemingly redundant code below masks the large nav mesh object so that the enemies cannot
    // walk outside of the map.
    // https://youtu.be/vQgLdFNrCN8?t=405
    Transform maskLeft, maskRight, maskTop, maskBottom;

    Func<Vector3, Transform> createMask = (v) => { 
      return Instantiate(
        maskPrefab, v * (map.size.x + map.maxSize.x) / 4.0f * map.tileSize, Quaternion.identity
      );
    };

    Func<Vector3> xScale = () => { 
      return new Vector3((map.size.x - map.maxSize.x) / 2.0f, 1, map.size.y) * map.tileSize;
    };

    Func<Vector3> yScale = () => { 
      return new Vector3(map.maxSize.x, 1, (map.maxSize.y - map.size.y) / 2.0f) * map.tileSize;
    };

    maskLeft = createMask(Vector3.left);
    maskLeft.localScale = xScale();
    maskLeft.parent = containerObject;

    maskRight = createMask(Vector3.right);
    maskRight.localScale = xScale();
    maskRight.parent = containerObject;

    maskTop = createMask(Vector3.forward);
    maskTop.localScale = yScale();
    maskTop.parent = containerObject;

    maskBottom = createMask(Vector3.back);
    maskBottom.localScale = yScale();
    maskBottom.parent = containerObject;
  }

  Coordinate GetRandomCoordinate() {
    // This cleaver trick ensures we never run out of coordinates.
    Coordinate coordinate = shuffledTileCoordinates.Dequeue();
    shuffledTileCoordinates.Enqueue(coordinate);

    return coordinate;
  }
}
