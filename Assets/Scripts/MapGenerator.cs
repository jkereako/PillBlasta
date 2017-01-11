using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(BoxCollider))]
public class MapGenerator: MonoBehaviour {
  public Map[] maps;
  public int mapIndex = 0;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navMeshFloor;
  public Transform navMeshMaskPrefab;

  Transform[,] tiles;
  Map map;

  const string containerName = "GeneratedMap";

  void Awake() {
    FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
  }

  public Map GetActiveMap() {
    return map;
  }

  public Transform[,] GetMapTiles() {
    return tiles;
  }

  public void OnNewWave(Wave wave, int index) {
    mapIndex = Mathf.Clamp(index, 0, maps.Length);
    GenerateMap();
  }

  public void GenerateMap() {
    map = maps[mapIndex];
    // First, immediately destroy the container object if it already exists.
    if (transform.FindChild(containerName)) {
      DestroyImmediate(transform.FindChild(containerName).gameObject);
    }

    // Next, create a new game object at run-time to contain all of the newly generated tiles.
    Transform containerObject;
    Queue<Coordinate> shuffledTileCoordinates;
    HashSet<Coordinate> openCoordinatesSet;

    containerObject = new GameObject(containerName).transform;
    containerObject.parent = transform;

    map.tileCoordinates = CreateTileCoordinates(map);

    tiles = CreateTiles(map, tilePrefab, containerObject);

    shuffledTileCoordinates = new Queue<Coordinate>(
      Utility.Shuffle(map.tileCoordinates, map.seed)
    );
      
    map.obstacleCoordinates = CreateObstacleCoordinates(
      map, shuffledTileCoordinates, obstaclePrefab, containerObject
    );

    // Find coordinates without an obstacle
    openCoordinatesSet = new HashSet<Coordinate>(map.tileCoordinates);
    map.openTileCoordinates = new Coordinate[openCoordinatesSet.Count];
    openCoordinatesSet.SymmetricExceptWith(map.obstacleCoordinates);
    openCoordinatesSet.CopyTo(map.openTileCoordinates);

    CreateMapMask(map, navMeshMaskPrefab, containerObject);

    GetComponent<BoxCollider>().size = new Vector3(
      map.size.x * map.tileSize, 0.5f, map.size.y * map.tileSize
    );

    navMeshFloor.localScale = new Vector3(map.maxSize.x, map.maxSize.y) * map.tileSize;
  }

  Coordinate[] CreateTileCoordinates(Map map) {
    List<Coordinate> coordinates = new List<Coordinate>();

    for (int x = 0; x < map.size.x; x++) {
      for (int y = 0; y < map.size.y; y++) {
        coordinates.Add(new Coordinate(x, y));
      }
    }
      
    return coordinates.ToArray();
  }

  Transform[,] CreateTiles(Map map, Transform prefab, Transform containerObject) {
    Transform[,] someTiles = new Transform[map.size.x, map.size.y];

    for (int x = 0; x < map.size.x; x++) {
      for (int y = 0; y < map.size.y; y++) {
        Vector3 position;
        Transform tile;

        position = map.CoordinateToPosition(new Coordinate(x, y));
        tile = Instantiate(prefab, position, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - map.tileSeparatorWidth) * map.tileSize;

        someTiles[x, y] = tile;
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }

    return someTiles;
  }

  Coordinate[] CreateObstacleCoordinates(Map map, Queue<Coordinate> coordinateQueue, Transform prefab, Transform containerObject) {
    System.Random random = new System.Random(map.seed);
    // Creates a 2D array of bools that is initialized to false.
    bool[,] obstacleMap = new bool[(int)map.size.x, (int)map.size.y];
    int limit = (int)(map.size.x * map.size.y * map.obstacleFill);
    int obstacleCount = 0;
    List<Coordinate> coordinates = new List<Coordinate>();

    // Generate the obstalces
    for (int i = 0; i < limit; i++) {
      Coordinate coordinate = Utility.CycleQueue(coordinateQueue);
      obstacleMap[coordinate.x, coordinate.y] = true;
      obstacleCount += 1;

      if (coordinate == map.center || !map.IsMapCompletelyAccessible(obstacleMap, obstacleCount)) {
        obstacleCount -= 1;
        obstacleMap[coordinate.x, coordinate.y] = false;
        continue;
      }

      coordinates.Add(coordinate);
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

    return coordinates.ToArray();
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
}
