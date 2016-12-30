using UnityEngine;

// Declare the file's dependencies
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
public class Player: LiveEntity {
  public float speed = 5;

  PlayerController playerController;
  WeaponController weaponController;
  Camera mainCamera;

  protected override void Start() {
    base.Start();
    playerController = GetComponent<PlayerController>();
    weaponController = GetComponent<WeaponController>();
    mainCamera = Camera.main;
  }

  void Update() {
    // `GetAxisRaw` returns the unmolested input value. We use it here to make the pill stop on a dime.
    Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    Vector3 velocity = movement.normalized * speed;
    playerController.Move(velocity);

    // The code below allows the player's "eyes" to follow the mouse movement.
    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    // Emulate the playing surface by creating a new, flat plane. This simplifies the code as we
    // don't have to depend on the actual in-game plane.
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    float rayLength;

    if (plane.Raycast(ray, out rayLength)) {
      Vector3 point = ray.GetPoint(rayLength);
      Debug.DrawLine(ray.origin, point, Color.red);
      playerController.LookAt(point);
    }

    if (Input.GetMouseButton(0)) {
      weaponController.FireWeapon();
    }
  }
}
