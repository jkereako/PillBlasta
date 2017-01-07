[System.Serializable]
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