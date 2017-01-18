using UnityEngine;

public class LiveEntity: MonoBehaviour, IDamageable {
  protected float health;
  protected bool isAlive;

  // This is basically a function pointer.
  public event System.Action OnDeath;

  protected virtual void Start() {
    isAlive = true;
  }

  public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 direction) {
    TakeDamage(damage);
  }

  public void TakeDamage(float damage) {
    health -= damage;

    if (health <= 0 && isAlive) {
      Die();
    }
  }

  // Adds an item to the context menu when right-clicking on the script in Unity. Selecting the menu
  // item "Kill" will immediately execute the method Die().
  [ContextMenu("Kill")]
  protected void Die() {
    isAlive = false;

    if (OnDeath != null) {
      OnDeath();
    }

    Destroy(gameObject);
  }
}
