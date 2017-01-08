using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  Wave currentWave;
  int waveIndex = 0;
  int spawnCount;
  int activeCount;
  float nextSpawnTime;
  MapGenerator mapGenerator;

  void Start() {
    mapGenerator = FindObjectOfType<MapGenerator>();

    NextWave();
  }

  void Update() {
    if (spawnCount <= 0 || Time.time < nextSpawnTime) {
      return;
    }

    spawnCount -= 1;
    nextSpawnTime = Time.time + currentWave.waitValue;

    StartCoroutine(Spawn());
  }

  IEnumerator Spawn() {
    Queue<Coordinate> coordinateQueue;
    const float delay = 1.0f;
    const float flashSpeed = 4.0f;
    float timer = 0.0f;
    coordinateQueue = new Queue<Coordinate>(
      Utility.Shuffle(mapGenerator.map.openTileCoordinates, mapGenerator.map.seed)
    );

    Coordinate coordinate = Utility.CycleQueue(coordinateQueue);
    Transform tile = mapGenerator.tiles[coordinate.x, coordinate.y];
    Material material = tile.GetComponent<Renderer>().material;
    Color initialColor = material.color;
    Color flashColor = Color.red;

    // Flash the tile
    while (timer < delay) {
      material.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(timer * flashSpeed, 1));
      timer += Time.deltaTime;
      yield return null;
    }

    Enemy spawn = Instantiate(enemy, tile.position + Vector3.up, Quaternion.identity);
    // Assign the delegate `OnEnemyDeath`
    spawn.OnDeath += OnEnemyDeath;
  }

  void OnEnemyDeath() {
    activeCount -= 1;

    if (activeCount == 0 && waveIndex < waves.Length) {
      NextWave();
    }
  }

  void NextWave() {
    Debug.Log("Wave: " + waveIndex);

    currentWave = waves[waveIndex];
    activeCount = spawnCount = currentWave.entityCount;
    waveIndex += 1;
  }
}
