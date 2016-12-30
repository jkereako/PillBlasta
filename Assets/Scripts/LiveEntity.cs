using UnityEngine;

public class LiveEntity : MonoBehaviour, IDamageable {
  public float initialHealth;
  protected float health;
  protected bool isAlive;

  protected virtual void Start() {
    isAlive = true;
    health = initialHealth;
  }

  public void TakeHit(float damage, RaycastHit hit) {
    health -= damage;

    if (health <= 0 && isAlive) {
      Die();
    }
  }

  protected void Die() {
    isAlive = false;
    Destroy(gameObject);
  }
}
