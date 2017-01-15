using UnityEngine;
using System.Collections;

public class MuzzleFlash: MonoBehaviour {
  public GameObject flash;
  public Sprite[] sprites;
  public SpriteRenderer[] spriteRenderers;
  public float animationDuration;

  public void Animate() {
    StartCoroutine(Flash());
  }

  IEnumerator Flash() {
    float time = 0.0f;
    int spriteIndex = Random.Range(0, sprites.Length);

    for (int i = 0; i < spriteRenderers.Length; i++) {
      spriteRenderers[i].sprite = sprites[spriteIndex];
    }

    flash.SetActive(true);

    while (time < animationDuration) {
      time += Time.deltaTime;
      yield return null;
    }

    flash.SetActive(false);
  }
}
