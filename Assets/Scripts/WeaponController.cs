using UnityEngine;
using System.Reflection;

public class WeaponController: MonoBehaviour {

  public Transform weaponHold;
  public Weapon initialWeapon;

  Weapon weapon;

  void Start() {
    if (initialWeapon != null) {
      EquipWeapon(initialWeapon);
    }
  }

  public void OnTriggerPull() {
    if (weapon != null) {
      weapon.OnTriggerPull();
    }
  }

  public void OnTriggerRelease() {
    if (weapon != null) {
      weapon.OnTriggerRelease();
    }
  }

  public void EquipWeapon(Weapon aWeapon) {
    if (weapon != null) {
      // Mark the current weapon for destruction
      Destroy(weapon);
    }

    weapon = Instantiate(aWeapon, weaponHold.position, weaponHold.rotation) as Weapon;
    // Bind the weapon as a child of the  to the `weaponHold` game object, which is a child of
    // player, so that when the player moves the gun moves also
    weapon.transform.parent = weaponHold;
  }
}
