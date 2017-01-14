using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Shell: MonoBehaviour {
  public float minForce;
  public float maxForce;
  public float lifetime = 4.0f;
  public float animationDuration = 2.0f;

  Rigidbody rigidBody;

  void Start() {
    float force = Random.Range(minForce, maxForce);
    rigidBody = GetComponent<Rigidbody>();
    rigidBody.AddForce(transform.right * force);
    rigidBody.AddTorque(Random.insideUnitSphere * force);

    StartCoroutine(Fade());
  }

  IEnumerator Fade() {
    yield return new WaitForSeconds(lifetime);

    float percentCompleted = 0.0f;
    float animationSpeed = 1.0f / animationDuration;
    Material material = GetComponent<Renderer>().material;
    Color initialColor = material.color;

    while (percentCompleted < 1.0f) {
      percentCompleted += Time.deltaTime * animationSpeed;
      material.color = Color.Lerp(initialColor, Color.clear, percentCompleted);
      yield return null;
    }

    Destroy(gameObject);
  }
}
