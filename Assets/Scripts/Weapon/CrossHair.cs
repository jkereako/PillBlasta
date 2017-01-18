using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour {

  public LayerMask targetMask;
  public SpriteRenderer crossHairs;
  public SpriteRenderer dot;
  public Color color;
  public Color highlightColor;

  void Start() {
    Cursor.visible = false;
    dot.color = color;
    crossHairs.color = color;
  }

  void Update() {
    // Rotate the crosshairs
    transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
  }

  public void DetectTargets(Ray ray) {
    if (Physics.Raycast(ray, 100, targetMask)) {
      dot.color = highlightColor;
    }
    else {
      dot.color = color;
    }
  }
}
