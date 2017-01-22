using UnityEngine;
using System.Reflection;

public class WeaponController: MonoBehaviour {
  public Transform weaponHold;
  public Weapon[] weapons;
  Weapon equippedWeapon;

  public void Aim(Vector3 point) {
    if (equippedWeapon != null) {
      equippedWeapon.Aim(point);
    }
  }

  public void Reload() {
    if (equippedWeapon != null) {
      equippedWeapon.Reload();
    }
  }

  public void OnTriggerPull() {
    if (equippedWeapon != null) {
      equippedWeapon.OnTriggerPull();
    }
  }

  public void OnTriggerRelease() {
    if (equippedWeapon != null) {
      equippedWeapon.OnTriggerRelease();
    }
  }

  public void EquipWeapon(Weapon aWeapon) {
    if (equippedWeapon != null) {
      // Mark the current weapon for destruction
      Destroy(equippedWeapon);
    }

    equippedWeapon = Instantiate(aWeapon, weaponHold.position, weaponHold.rotation) as Weapon;
    // Bind the weapon as a child of the  to the `weaponHold` game object, which is a child of
    // player, so that when the player moves the gun moves also
    equippedWeapon.transform.parent = weaponHold;
  }
}
