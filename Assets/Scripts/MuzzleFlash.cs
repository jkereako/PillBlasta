using UnityEngine;

public class MuzzleFlash: MonoBehaviour {
  public GameObject flash;
  public Sprite[] sprites;
  public SpriteRenderer[] spriteRenderers;
  public float flashTime;

  void Start() {
    Deactivate();
  }

  public void Activate() {
    int spriteIndex = Random.Range(0, sprites.Length);

    for (int i = 0; i < spriteRenderers.Length; i++) {
      spriteRenderers[i].sprite = sprites[spriteIndex];
    }

    flash.SetActive(true);

    Invoke("Deactivate", flashTime);
  }

  public void Deactivate() {
    flash.SetActive(false);
  }
}
