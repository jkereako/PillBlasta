using UnityEngine;

public class Projectile: MonoBehaviour {
  public LayerMask collisionMask;
  public float speed;

  void Update() {
    float distance = Time.deltaTime * speed;
    CheckCollision(distance);
    transform.Translate(Vector3.forward * distance);
  }

  void CheckCollision(float distance) {
    Ray ray = new Ray(transform.position, transform.forward);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, distance, collisionMask, QueryTriggerInteraction.Collide)) {
      OnObjectHit(hit);
    }
  }

  void OnObjectHit(RaycastHit hit) {
    GameObject.Destroy(gameObject);
  }
}
