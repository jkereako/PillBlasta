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
  Vector3 velocity;

  void Start() {
    Initialize();
  }

  void Update() {
    // Reset the position of the weapon.
    Vector3 smoothDamp;
    smoothDamp = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref velocity, 0.1f);
    transform.localPosition = smoothDamp;
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

  public void Aim(Vector3 point) {
    transform.LookAt(point);
  }

  void Fire() {
    Projectile aProjectile;
    MuzzleFlash muzzleFlash = GetComponent<MuzzleFlash>();
    nextShotTime = Time.time + fireRate / 1000;
    aProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
    aProjectile.SetSpeed(muzzleVelocity);

    Instantiate(shell, ejector.position, ejector.rotation);
    muzzleFlash.Animate();

    AnimateRecoil();
  }

  void AnimateRecoil() {
    // Move the weapon backward
    transform.localPosition -= Vector3.forward * 0.2f;
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
