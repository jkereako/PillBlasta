using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  LiveEntity playerEntity;
  Transform player;

  Wave currentWave;
  int waveIndex = 0;
  int spawnCount;
  int activeCount;
  float nextSpawnTime;
  bool isActive = true;
  MapGenerator mapGenerator;
  PlayerCampManager playerCampManager;

  void Start() {
    playerEntity = FindObjectOfType<Player>();
    player = playerEntity.transform;
    mapGenerator = FindObjectOfType<MapGenerator>();
    playerCampManager = new PlayerCampManager(player);

    playerEntity.OnDeath += OnPlayerDeath;

    NextWave();
  }

  void Update() {
    if (!isActive || spawnCount <= 0 || Time.time < nextSpawnTime) {
      return;
    }

    spawnCount -= 1;
    nextSpawnTime = Time.time + currentWave.waitValue;

    StartCoroutine(Spawn());
  }

  IEnumerator Spawn() {
    const float delay = 1.0f;
    const float flashSpeed = 4.0f;
    float timer = 0.0f;
    Transform tile;
    Coordinate coordinate;

    // Prevent the player from chilling in a corner.
    if (playerCampManager.isPlayerCamped) {
      Debug.Log("Player is camped");
      coordinate = mapGenerator.map.PositionToCoordinate(player.position);
    }
    else {
      Queue<Coordinate> coordinateQueue;
      coordinateQueue = new Queue<Coordinate>(
        Utility.Shuffle(mapGenerator.map.openTileCoordinates, mapGenerator.map.seed)
      );

      coordinate = Utility.CycleQueue(coordinateQueue);
    }

    tile = mapGenerator.tiles[coordinate.x, coordinate.y];

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

  void OnPlayerDeath() {
    isActive = false;
  }

  void NextWave() {
    Debug.Log("Wave: " + waveIndex);

    currentWave = waves[waveIndex];
    activeCount = spawnCount = currentWave.entityCount;
    waveIndex += 1;
  }
}
