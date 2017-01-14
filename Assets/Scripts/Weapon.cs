using UnityEngine;

[RequireComponent(typeof(MuzzleFlash))]
public class Weapon: MonoBehaviour {
  public Transform muzzle;
  public Transform ejector;
  public Projectile projectile;
  public Transform shell;

  public float fireRate = 100;
  public float muzzleVelocity = 35;

  float nextShotTime;
  MuzzleFlash muzzleFlash;

  public void Start() {
    muzzleFlash = GetComponent<MuzzleFlash>();
  }

  public void Fire() {
    if (Time.time < nextShotTime) {
      return;
    }

    Projectile aProjectile;

    nextShotTime = Time.time + fireRate / 1000;
    aProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
    aProjectile.SetSpeed(muzzleVelocity);

    Instantiate(shell, ejector.position, ejector.rotation);
    muzzleFlash.Activate();
  }
}
