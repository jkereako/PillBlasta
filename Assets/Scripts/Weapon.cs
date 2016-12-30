using UnityEngine;

public class Weapon: MonoBehaviour {
  public Transform muzzle;
  public Projectile projectile;
  public float fireRate = 100;
  public float muzzleVelocity = 35;

  float nextShotTime;

  public void Fire() {
    if (Time.time < nextShotTime) {
      return;
    }

    nextShotTime = Time.time + fireRate / 1000;
    Projectile aProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
    aProjectile.SetSpeed(muzzleVelocity);
  }
}
