using UnityEngine;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  Wave currentWave;
  int waveIndex = 0;
  int spawnCount;
  int activeCount;
  float nextSpawnTime;

  void Start() {
    NextWave();
  }

  void Update() {
    if (spawnCount <= 0 || Time.time < nextSpawnTime) {
      return;
    }

    spawnCount -= 1;
    nextSpawnTime = Time.time + currentWave.waitValue;

    Enemy spawn = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
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
