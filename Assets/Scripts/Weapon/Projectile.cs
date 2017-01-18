using UnityEngine;

public class Projectile: MonoBehaviour {
  public LayerMask collisionMask;

  float speed;
  const float damage = 1.0f;
  const float lifeTime = 2.0f;
  const float targetSurfaceThickness = 0.1f;

  void Start() {
    Destroy(gameObject, lifeTime);

    Collider[] collisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

    if (collisions.Length > 0) {
      OnObjectHit(collisions[0]);
    }
  }

  void Update() {
    float distance = Time.deltaTime * speed;
    CheckCollision(distance);
    transform.Translate(Vector3.forward * distance);
  }

  public void SetSpeed(float newSpeed) {
    speed = newSpeed;
  }

  void CheckCollision(float distance) {
    Ray ray = new Ray(transform.position, transform.forward);
    RaycastHit hit;
    bool result = Physics.Raycast(
                    ray, 
                    out hit,
                    distance + targetSurfaceThickness,
                    collisionMask, 
                    QueryTriggerInteraction.Collide
                  );
      
    if (result) {
      OnObjectHit(hit);
    }
  }

  void OnObjectHit(RaycastHit hit) {
    IDamageable damageable = hit.collider.GetComponent<IDamageable>();

    if (damageable != null) {
      damageable.TakeHit(damage, hit.point, transform.forward);
    }

    Destroy(gameObject);
  }

  void OnObjectHit(Collider aCollider) {
    IDamageable damageable = aCollider.GetComponent<IDamageable>();

    if (damageable != null) {
      damageable.TakeHit(damage, transform.position, transform.forward);
    }

    Destroy(gameObject);
  }
}
