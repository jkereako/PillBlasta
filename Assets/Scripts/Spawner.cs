using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  public event System.Action<Wave, int> OnNewWave;

  LiveEntity playerEntity;
  Transform player;
  MapGenerator mapGenerator;
  PlayerCampManager playerCampManager;
  int waveIndex;
  /// <summary>
  /// The number of spawned entities. The limit is the int `entityCount` in the struct Wave.
  /// </summary>
  int spawnedEntitiesCount;
  /// <summary>
  /// The number of surviving enemies. A new wave will spawn once this number reaches 0.
  /// </summary>
  int activeEntitiesCount;
  float nextSpawnTime;
  bool shouldStopSpawning;

  void Start() {
    playerEntity = FindObjectOfType<Player>();
    player = playerEntity.transform;
    mapGenerator = FindObjectOfType<MapGenerator>();
    playerCampManager = new PlayerCampManager(player);

    playerEntity.OnDeath += OnPlayerDeath;

    // Load the first wave at index 0.
    LoadNewWave(0);
  }

  void Update() {
    if (shouldStopSpawning || spawnedEntitiesCount >= waves[waveIndex].entityCount ||
        Time.time < nextSpawnTime) {
      return;
    }

    Spawn();

    activeEntitiesCount = spawnedEntitiesCount += 1;
    nextSpawnTime = Time.time + waves[waveIndex].delay;
  }

  void Spawn() {
    Transform tile;
    Enemy spawn;
    tile = FindSpawnableTile(mapGenerator.GetActiveMap(), player.position, playerCampManager);

    StartCoroutine(FlashTile(tile, Color.red));

    spawn = Instantiate(enemy, tile.position + Vector3.up, Quaternion.identity);
    // Assign the delegate `OnEnemyDeath`
    spawn.OnDeath += OnEnemyDeath;
  }

  Transform FindSpawnableTile(Map map, Vector3 playerPosition, PlayerCampManager aPlayerCampManager) {
    Coordinate coordinate;

    // Prevent the player from chilling in a corner.
    if (aPlayerCampManager.isPlayerCamped) {
      coordinate = map.CoordinateForPosition(playerPosition);
    }
    else {
      Queue<Coordinate> coordinateQueue;
      coordinateQueue = new Queue<Coordinate>(
        Utility.Shuffle(map.openTileCoordinates, map.seed)
      );

      coordinate = Utility.CycleQueue(coordinateQueue);
    }

    return mapGenerator.GetMapTiles()[coordinate.x, coordinate.y];
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
    activeEntitiesCount -= 1;

    // If all of the enemies have been killed and the wave index is not out of range, then load the
    // next wave.
    if (activeEntitiesCount == 0 && waveIndex < waves.Length - 1) {
      waveIndex += 1;
      LoadNewWave(waveIndex);
    }
  }

  void OnPlayerDeath() {
    shouldStopSpawning = true;
  }

  void LoadNewWave(int aWaveIndex) {
    activeEntitiesCount = spawnedEntitiesCount = 0;

    if (OnNewWave != null) {
      OnNewWave(waves[aWaveIndex], aWaveIndex);
    }
  }
}
