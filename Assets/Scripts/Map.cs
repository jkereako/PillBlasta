using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Map {
  public Coordinate size;
  public Vector2 maxSize;
  public int seed;
  [Range(0, 1)]
  public float tileSeparatorWidth;
  public float tileSize;
  public Obstacle obstacleData;
  [Range(0, 1)]
  public float obstacleFill;

  [HideInInspector]
  public Coordinate[] tileCoordinates;
  [HideInInspector]
  public Coordinate[] obstacleCoordinates;
  [HideInInspector]
  public Coordinate[] openTileCoordinates;

  public Coordinate centerCoordinate {
    get {
      return new Coordinate(size.x / 2, size.y / 2);
    }
  }

  public Vector3 centerPosition {
    get {
      return PositionForCoordinate(centerCoordinate);
    }
  }

  // This is an implementation of the Flood-fill 4 algorithm.
  public bool IsMapCompletelyAccessible(bool[,] visitedObstacles, int obstacleCount) {
    // Boolean arrays are initialized with false
    bool[,] visitedNeighbors = new bool[size.x, size.y];
    Queue<Coordinate> queue = new Queue<Coordinate>();
    int accessibleTileCount = 1;
    int walkableTileCount = size.x * size.y - obstacleCount;

    Coordinate[] neighborOffsets = { 
      new Coordinate(-1, 0), // Left 
      new Coordinate(0, 1),  // Top
      new Coordinate(1, 0),  // Right
      new Coordinate(0, -1)  // Bottom
    };

    queue.Enqueue(centerCoordinate);
    visitedNeighbors[centerCoordinate.x, centerCoordinate.y] = true;

    while (queue.Count > 0) {
      Coordinate tile = queue.Dequeue();

      foreach (Coordinate neighborOffset in neighborOffsets) {
        Coordinate neighbor = tile + neighborOffset;

        // This condition checks that we stay within the bounds of our map.
        if ((neighbor.x >= 0 && neighbor.x < size.x) && (neighbor.y >= 0 && neighbor.y < size.y)) {
          if (visitedNeighbors[neighbor.x, neighbor.y] || visitedObstacles[neighbor.x, neighbor.y]) {
            continue;
          }

          visitedNeighbors[neighbor.x, neighbor.y] = true;
          queue.Enqueue(neighbor);
          accessibleTileCount += 1;
        }
      }
    }

    return walkableTileCount == accessibleTileCount;
  }

  public Vector3 PositionForCoordinate(Coordinate coordinate) {
    return new Vector3(-size.x / 2.0f + 0.5f + coordinate.x, 0, -size.y / 2.0f + 0.5f + coordinate.y) * tileSize;
  }

  public Coordinate CoordinateForPosition(Vector3 position) {
    int x = Mathf.RoundToInt(position.x / tileSize + (size.x - 1) / 2.0f);
    // NOTE: `position.z` is intentional.
    int y = Mathf.RoundToInt(position.z / tileSize + (size.y - 1) / 2.0f);
    x = Mathf.Clamp(x, 0, size.x);
    y = Mathf.Clamp(y, 0, size.y);

    return new Coordinate(x, y);
  }
}
