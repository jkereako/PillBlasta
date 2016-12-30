using UnityEngine;

public class Spawner: MonoBehaviour {
  public Enemy enemy;
  public Wave[] waves;

  Wave currentWave;
  int waveIndex;
  int enemiesRemaining;
  float nextSpawnTime;

  void Start() {
    NextWave();
  }

  void Update() {
    if (enemiesRemaining > 0 && Time.time > nextSpawnTime) {
      enemiesRemaining -= 1;
      nextSpawnTime = Time.time + currentWave.waitValue;

      Enemy spawn = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
    }
  }

  void NextWave() {
    waveIndex += 1;
    currentWave = waves[waveIndex - 1];

    enemiesRemaining = currentWave.enemyCount;
  }

  [System.Serializable]
  public class Wave {
    public int enemyCount;
    public float waitValue;
  }

}
