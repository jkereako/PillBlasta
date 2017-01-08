using UnityEngine;
using System.Collections.Generic;
using System.Net;

[System.Serializable]
public class Map {
  public Coordinate size;
  public Vector2 maxSize;
  public int seed;
  public float tileSeparatorWidth;
  public float tileSize;
  public Obstacle obstacleData;
  [Range(0, 1)]
  public float obstaclePercent;

  public Coordinate center {
    get {
      return new Coordinate(size.x / 2, size.y / 2);
    }
  }

  // This is an implementation of the Flood-fill 4 algorithm.
  public bool IsMapCompletelyAccessible(bool[,] obstacleMap, int obstacleCount) {
    // Boolean arrays are initialized with false
    bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
    Queue<Coordinate> queue = new Queue<Coordinate>();
    int accessibleTileCount = 0;

    queue.Enqueue(center);
    mapFlags[center.x, center.y] = true;

    while (queue.Count > 0) {
      Coordinate tile = queue.Dequeue();

      for (int x = -1; x < 2; x++) {
        for (int y = -1; y < 2; y++) {
          // This condition makes this a Flood-fill 4 algorithm. That is, we ignore the diagonal 
          // neighbors.
          if (x != 0 && y != 0) {
            continue;
          }

          Coordinate neighbor = new Coordinate(tile.x + x, tile.y + y);

          // `obstacleMap.GetLength(0)` returns the number of elements for the 0 dimension. This
          // condition checks that we stay within the bounds of our map.
          if (neighbor.x >= 0 && neighbor.x < obstacleMap.GetLength(0) &&
              neighbor.y >= 0 && neighbor.y < obstacleMap.GetLength(1)) {

            if (mapFlags[neighbor.x, neighbor.y] || obstacleMap[neighbor.x, neighbor.y]) {
              continue;
            }

            mapFlags[neighbor.x, neighbor.y] = true;
            queue.Enqueue(neighbor);
            accessibleTileCount += 1;
          }
        }
      }
    }

    int expectedAccessibleTileCount = (int)(size.x * size.y - obstacleCount);

    return expectedAccessibleTileCount == accessibleTileCount;
  }

  public Vector3 CoordinateToPosition(Coordinate coordinate) {
    return new Vector3(-size.x / 2.0f + 0.5f + coordinate.x, 0, -size.y / 2.0f + 0.5f + coordinate.y) * tileSize;
  }
}