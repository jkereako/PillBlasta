using UnityEngine;

public enum FireMode {
  Automatic,
  Burst,
  Single}

;

[RequireComponent(typeof(MuzzleFlash))]
public class Weapon: MonoBehaviour {
  public FireMode fireMode;
  public Transform muzzle;
  public Transform ejector;
  public Projectile projectile;
  public Transform shell;

  public int burstCount = 3;
  public float fireRate = 100;
  public float muzzleVelocity = 35;

  float nextShotTime;
  int shotsRemaining;

  void Start() {
    Initialize();
  }

  public void OnTriggerPull() {
    if (shotsRemaining == 0 || Time.time < nextShotTime) {
      return;
    }

    Fire();

    switch (fireMode) {
    case FireMode.Single:
    case FireMode.Burst:
      shotsRemaining -= 1;
      break;
    }
  }

  public void OnTriggerRelease() {
    Initialize();
  }

  void Fire() {
    Projectile aProjectile;
    MuzzleFlash muzzleFlash = GetComponent<MuzzleFlash>();
    nextShotTime = Time.time + fireRate / 1000;
    aProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
    aProjectile.SetSpeed(muzzleVelocity);

    Instantiate(shell, ejector.position, ejector.rotation);
    muzzleFlash.Animate();
  }

  void Initialize() {
    switch (fireMode) {
    case FireMode.Single:
      shotsRemaining = 1;
      break;
    case FireMode.Burst:
      shotsRemaining = burstCount;
      break;
    case FireMode.Automatic:
      shotsRemaining = -1;
      break;
    }
  }
}
