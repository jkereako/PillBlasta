using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour {
  public Image fadePlane;
  public GameObject gameOverUI;
  public RectTransform banner;
  public Text title;
  public Text subtitle;

  void Awake() {
    FindObjectOfType<Player>().OnDeath += OnGameOver;
    FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
  }

  public void StartNewGame() {
    SceneManager.LoadScene("Main");
  }

  void OnNewWave(Wave wave, int waveCount) {
    title.text = "WAVE " + (waveCount + 1);
    subtitle.text = wave.entityCount + " enemies";
    StartCoroutine(AnimateBanner(1.5f, 2.0f));
  }

  void OnGameOver() {
    gameOverUI.SetActive(true);
    StartCoroutine(Fade(Color.clear, Color.black, 1.0f));
  }

  IEnumerator AnimateBanner(float speed, float delay) {
    const int offScreenYPosition = -460;
    const int onScreenYPosition = -200;
    int direction = 1;
    float percentCompleted = 0.0f;
    float restartExecutionTime = Time.time + 1 / speed + delay;

    while (percentCompleted >= 0) {
      percentCompleted += Time.deltaTime * speed * direction;

      if (percentCompleted >= 1) {
        percentCompleted = 1;

        if (Time.time > restartExecutionTime) {
          direction = -1;
        }
      }

      banner.anchoredPosition = Vector2.up * Mathf.Lerp(
        offScreenYPosition, onScreenYPosition, percentCompleted
      );

      yield return null;
    }
  }

  IEnumerator Fade(Color fromColor, Color toColor, float duration) {
    float speed = 1.0f / duration;
    float percentCompleted = 0.0f;

    while (percentCompleted < 1.0f) {
      percentCompleted += Time.deltaTime * speed;
      fadePlane.color = Color.Lerp(fromColor, toColor, percentCompleted);
      yield return null;
    }
  }
}
