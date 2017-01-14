using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIController : MonoBehaviour {
  public Image fadePlane;
  public GameObject gameOverUI;

  void Awake() {
    FindObjectOfType<Player>().OnDeath += OnGameOver;
  }

  void OnGameOver() {
    gameOverUI.SetActive(true);
    StartCoroutine(Fade(Color.clear, Color.black, 1.0f));
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
