using UnityEngine;
using System.Runtime.Remoting;
using System.Collections;
using UnityEngine.Audio;

public enum FireMode {
  SemiAutomatic,
  ThreeRoundBurst,
  FullyAutomatic}

;

[RequireComponent(typeof(MuzzleFlash))]
public class Weapon: MonoBehaviour {
  [Header("Weapon attributes")]
  public FireMode fireMode;
  public float fireRate = 100;
  public int magazineSize = 12;
  public float muzzleVelocity = 35;

  [Header("Referenced objects")]
  public Transform muzzle;
  public Transform ejector;
  public Projectile projectile;
  public Transform shell;
  public AudioClip fireSoundEffect;
  public AudioClip reloadSoundEffect;

  float nextShotTime;
  int shotsLeftInBurst;
  int shotsLeftInMagazine;
  bool isReloading;
  float angleDampaningVelocity;
  Vector3 postitionDampaningVelocity;
  float recoilAngle;

  void Start() {
    shotsLeftInMagazine = magazineSize;

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

    if (!isReloading && shotsLeftInMagazine == 0) {
      Reload();
    }
  }

  public void OnTriggerPull() {
    // Multiplying `shotsLeftInMagazine` by `shotsLeftInBurst` is a concise way to determine if
    // either value is 0.
    if (isReloading || shotsLeftInMagazine * shotsLeftInBurst == 0 || Time.time < nextShotTime) {
      return;
    }

    Fire();

    shotsLeftInMagazine -= 1;

    switch (fireMode) {
    case FireMode.SemiAutomatic:
    case FireMode.ThreeRoundBurst:
      shotsLeftInBurst -= 1;
      break;
    }
  }

  public void OnTriggerRelease() {
    Initialize();
  }

  public void Aim(Vector3 point) {
    transform.LookAt(point);
  }

  public void Reload() {
    AudioManager.instance.PlaySoundEffect(reloadSoundEffect, transform.position);
    StartCoroutine(AnimateReload());
  }

  void Fire() {
    Projectile aProjectile;
    MuzzleFlash muzzleFlash = GetComponent<MuzzleFlash>();
    nextShotTime = Time.time + fireRate / 1000;
    aProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
    aProjectile.SetSpeed(muzzleVelocity);

    Instantiate(shell, ejector.position, ejector.rotation);
    muzzleFlash.Animate();
    AudioManager.instance.PlaySoundEffect(fireSoundEffect, transform.position);

    AnimateRecoil();
  }

  void AnimateRecoil() {
    recoilAngle += 8.0f;
    Mathf.Clamp(recoilAngle, 0, 30);
    // Move the weapon backward
    transform.localPosition -= Vector3.forward * 0.2f;
  }

  IEnumerator AnimateReload() {
    const float reloadTime = 0.5f;
    const float speed = 1.0f / reloadTime;
    float percentCompleted = 0.0f;
    Vector3 initialRoration = transform.localEulerAngles;

    isReloading = true;

    yield return new WaitForSeconds(0.2f);

    while (percentCompleted <= 1.0f) {
      percentCompleted += Time.deltaTime * speed;
      float interpolation = (-Mathf.Pow(percentCompleted, 2) + percentCompleted) * 4;
      float angle = Mathf.Lerp(0, 30.0f, interpolation);
      transform.localEulerAngles = initialRoration + Vector3.left * angle;

      yield return null;
    }

    shotsLeftInMagazine = magazineSize;
    isReloading = false;
  }

  void Initialize() {
    switch (fireMode) {
    case FireMode.SemiAutomatic:
      shotsLeftInBurst = 1;
      break;
    
    case FireMode.ThreeRoundBurst:
      shotsLeftInBurst = 3;
      break;

    case FireMode.FullyAutomatic:
      shotsLeftInBurst = -1;
      break;
    }
  }
}
