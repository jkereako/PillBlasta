using UnityEngine;
using System.Collections.Generic;

public class MapGenerator: MonoBehaviour {
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Vector2 mapSize;
  [Range(0, 1)]
  public float outLinePercent;
  [Range(0, 1)]
  public float obstaclePercent;
  List<Coordinate> tileCoordinates;
  Queue<Coordinate> shuffledTileCoordinates;
  Coordinate mapCenter;
  public int seed = 10;

  public struct Coordinate {
    public int x;
    public int y;

    public Coordinate(int xCoordinate, int yCoordinate) {
      x = xCoordinate;
      y = yCoordinate;
    }

    public static bool operator ==(Coordinate c1, Coordinate  c2) {
      return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Coordinate c1, Coordinate  c2) {
      return c1.x != c2.x || c1.y != c2.y;
    }
  }

  void Start() {
    GenerateMap();
  }

  public void GenerateMap() {
    const string containerName = "GeneratedMap";

    tileCoordinates = new List<Coordinate>();
    for (int x = 0; x < mapSize.x; x++) {
      for (int y = 0; y < mapSize.y; y++) {
        tileCoordinates.Add(new Coordinate(x, y));
      }
    }

    mapCenter = new Coordinate((int)mapSize.x / 2, (int)mapSize.y / 2);
    shuffledTileCoordinates = new Queue<Coordinate>(Utility.Shuffle(tileCoordinates.ToArray(), seed));

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
        tile.localScale = Vector3.one * (1 - outLinePercent);
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }

    bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
    int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
    int currentObstacleCount = 0;
    // Generate the obstalces
    for (int i = 0; i < obstacleCount; i++) {
      Coordinate coordinate = GetRandomCoordinate();
      obstacleMap[coordinate.x, coordinate.y] = true;
      currentObstacleCount += 1;

      if (coordinate == mapCenter || !IsMapCompletelyAccessible(obstacleMap, i)) {
        currentObstacleCount -= 1;
        obstacleMap[coordinate.x, coordinate.y] = false;
        continue;
      }

      Vector3 obstaclePosition = CoordinateToPosition(coordinate.x, coordinate.y);
      Transform obstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
      obstacle.parent = containerObject;
    }
  }

  // This is an implementation of the Flood-fill 4 algorithm.
  bool IsMapCompletelyAccessible(bool[,] obstacleMap, int obstacleCount) {
    bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
    Queue<Coordinate> queue = new Queue<Coordinate>();

    queue.Enqueue(mapCenter);
    mapFlags[mapCenter.x, mapCenter.y] = true;
    int accessibleTileCount = 0;

    while (queue.Count > 0) {
      Coordinate tile = queue.Dequeue();

      for (int x = -1; x < 2; x++) {
        for (int y = -1; y < 2; y++) {
          int neighborX = tile.x + x;
          int neighborY = tile.y + y;

          if (x == 0 || y == 0) {
            if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) &&
                neighborY >= 0 && neighborY < obstacleMap.GetLength(1)) {

              if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY]) {
                mapFlags[neighborX, neighborY] = true;
                queue.Enqueue(new Coordinate(neighborX, neighborY));
                accessibleTileCount += 1;
              }
            }
          }
        }
      }
    }

    int expectedAccessibleTileCount = (int)(mapSize.x * mapSize.y - obstacleCount);

    return expectedAccessibleTileCount == accessibleTileCount;
  }

  Vector3 CoordinateToPosition(int x, int y) {
    return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
  }

  Coordinate GetRandomCoordinate() {
    // This cleaver trick ensures we never run out of coordinates.
    Coordinate coordinate = shuffledTileCoordinates.Dequeue();
    shuffledTileCoordinates.Enqueue(coordinate);

    return coordinate;
  }

}
