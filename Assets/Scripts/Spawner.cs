using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

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

    Spawn();
  }

  void Spawn() {
    Transform tile;
    Enemy spawn;
    tile = FindSpawnableTile(mapGenerator.map, player.position, playerCampManager);

    StartCoroutine(FlashTile(tile, Color.red));

    spawn = Instantiate(enemy, tile.position + Vector3.up, Quaternion.identity);
    // Assign the delegate `OnEnemyDeath`
    spawn.OnDeath += OnEnemyDeath;
  }

  Transform FindSpawnableTile(Map map, Vector3 playerPosition, PlayerCampManager aPlayerCampManager) {
    Coordinate coordinate;

    // Prevent the player from chilling in a corner.
    if (aPlayerCampManager.isPlayerCamped) {
      coordinate = map.PositionToCoordinate(playerPosition);
    }
    else {
      Queue<Coordinate> coordinateQueue;
      coordinateQueue = new Queue<Coordinate>(
        Utility.Shuffle(map.openTileCoordinates, map.seed)
      );

      coordinate = Utility.CycleQueue(coordinateQueue);
    }

    return mapGenerator.tiles[coordinate.x, coordinate.y];
  }

  IEnumerator FlashTile(Transform tile, Color flashColor) {
    const float delay = 1.0f;
    const float flashSpeed = 4.0f;
    float timer = 0.0f;
    Material material;
    Color initialColor;

    material = tile.GetComponent<Renderer>().material;
    initialColor = material.color;
    // Flash the tile
    while (timer < delay) {
      material.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(timer * flashSpeed, 1));
      timer += Time.deltaTime;
      yield return null;
    }
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
