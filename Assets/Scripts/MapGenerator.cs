using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;

public class MapGenerator: MonoBehaviour {
  public Transform tilePrefab;
  public Vector2 mapSize;
  [Range(0, 1)]
  public float outLinePercent;

  void Start() {
    GenerateMap();
  }

  public void GenerateMap() {
    const string containerName = "GeneratedMap";

    // First, immediately destroy the container object if it already exists.
    if (transform.FindChild(containerName)) {
      DestroyImmediate(transform.FindChild(containerName).gameObject);
    }
      
    // Next, create a new game object at run-time to contain all of the newly generated tiles.
    Transform containerObject = new GameObject(containerName).transform;
    containerObject.parent = transform;

    for (int x = 0; x < mapSize.x; x++) {
      for (int y = 0; y < mapSize.y; y++) {
        Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
        Transform tile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
        tile.localScale = Vector3.one * (1 - outLinePercent);
        // Associate the tiles with the generated map.
        tile.parent = containerObject;
      }
    }
  }
}
