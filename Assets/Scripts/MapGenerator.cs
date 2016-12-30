using UnityEngine;
using System.Collections.Generic;

public class MapGenerator: MonoBehaviour {
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Vector2 mapSize;
  [Range(0, 1)]
  public float outLinePercent;
  List<Coordinate> tileCoordinates;
  Queue<Coordinate> shuffledTileCoordinates;
  public int seed = 10;

  public struct Coordinate {
    public int x;
    public int y;

    public Coordinate(int xCoordinate, int yCoordinate) {
      x = xCoordinate;
      y = yCoordinate;
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

    shuffledTileCoordinates = new Queue<Coordinate>(Utility.Shuffle(tileCoordinates.ToArray(), seed));
    // First, immediately destroy the container object if it already exists.
    if (transform.FindChild(containerName)) {
      DestroyImmediate(transform.FindChild(containerName).gameObject);
    }
      
    // Next, create a new game object at run-time to contain all of the newly generated tiles.
    Transform containerObject = new GameObject(containerName).transform;
    containerObject.parent = transform;

    for (int x = 0; x < mapSize.x; x++) {
      for (int y = 0; y < mapSize.y; y++) {
        Vector3 tilePosition = CoordinateToPosition(x, y);
        Transform tile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - outLinePercent);
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }

    const int obstacleCount = 10;

    for (int i = 0; i < obstacleCount; i++) {
      Coordinate coordinate = GetRandomCoordinate();
      Vector3 obstaclePosition = CoordinateToPosition(coordinate.x, coordinate.y);
      Transform obstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
      obstacle.parent = containerObject;
    }
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
