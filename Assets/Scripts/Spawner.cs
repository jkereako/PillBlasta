using UnityEngine;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  Wave currentWave;
  int waveIndex;
  int spawnCount;
  int activeCount;
  float nextSpawnTime;

  void Start() {
    NextWave();
  }

  void Update() {
    if (spawnCount > 0 && Time.time > nextSpawnTime) {
      spawnCount -= 1;
      nextSpawnTime = Time.time + currentWave.waitValue;

      Enemy spawn = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
      // Assign the delegate `OnEnemyDeath`
      spawn.OnDeath += OnEnemyDeath;
    }
  }

  void OnEnemyDeath() {
    activeCount -= 1;

    if (activeCount == 0) {
      NextWave();
    }
  }

  void NextWave() {
    Debug.Log("Wave: " + waveIndex);

    if (waveIndex > waves.Length) {
      return;
    }

    currentWave = waves[waveIndex];
    activeCount = spawnCount = currentWave.enemyCount;
    waveIndex += 1;
  }

  [System.Serializable]
  public class Wave {
    public int enemyCount;
    public float waitValue;
  }

}
