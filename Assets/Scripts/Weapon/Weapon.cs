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
  float angleDampaningVelocity;
  Vector3 postitionDampaningVelocity;
  float recoilAngle;

  void Start() {
    Initialize();
  }

  void LateUpdate() {
    // Reset the position of the weapon.
    Vector3 postitionDampaning;
    postitionDampaning = Vector3.SmoothDamp(
      transform.localPosition, Vector3.zero, ref postitionDampaningVelocity, 0.1f
    );
    recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref angleDampaningVelocity, 0.1f);

    transform.localPosition = postitionDampaning;
    transform.localEulerAngles += Vector3.left * recoilAngle;   
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
    recoilAngle += 8.0f;
    Mathf.Clamp(recoilAngle, 0, 30);
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
