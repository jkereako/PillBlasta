using UnityEngine;

// Declare the file's dependencies
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
public class Player: LiveEntity {
  public EntityTrait trait;
  public CrossHair crossHair;
  PlayerController playerController;
  WeaponController weaponController;
  Camera mainCamera;

  void Awake() {
    playerController = GetComponent<PlayerController>();
    weaponController = GetComponent<WeaponController>();
    mainCamera = Camera.main;

    // Set entity traits
    GetComponent<Renderer>().material.color = trait.color;
    health = trait.health;
  }

  void Update() {
    // The code below allows the player's "eyes" to follow the mouse movement.
    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //  Debug.DrawLine(ray.origin, point, Color.red);

    // Emulate the playing surface by creating a new, flat plane. This simplifies the code as we
    // don't have to depend on the actual in-game plane.
    //
    // `Vector3.up` Shorthand for writing `Vector3(0, 1, 0)`.
    Plane plane = new Plane(Vector3.up, Vector3.up * weaponController.weaponHold.position.y);
    float rayLength;

    // This function sets enter to the distance along the ray, where it intersects the plane. If the
    // ray is parallel or pointing in the opposite direction to the plane then `Raycast()` is false
    // and `rayLength` is 0 and negative, respectively.
    if (!plane.Raycast(ray, out rayLength)) {
      return;
    }
     
    const int threshold = 6;
    Vector3 point = ray.GetPoint(rayLength);
    Vector2 newPoint = new Vector2(point.x, point.z);
    Vector2 newPosition = new Vector2(transform.position.x, transform.position.z);

    playerController.LookAt(point);
         
    if ((newPoint - newPosition).sqrMagnitude > threshold) {
      weaponController.Aim(point);
    }

    crossHair.transform.position = point;
    crossHair.DetectTargets(ray);

    ScanInput();
    Move();
  }

  void ScanInput() {
    if (Input.GetMouseButton(0)) {
      weaponController.OnTriggerPull();
    }
    else if (Input.GetMouseButtonUp(0)) {
      weaponController.OnTriggerRelease();
    }

    if (Input.GetKeyDown(KeyCode.R)) {
      weaponController.Reload();
    }
  }

  void Move() {
    // `GetAxisRaw` returns the unmolested input value. We use it here to make the pill stop on a 
    // dime.
    Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    Vector3 velocity = movement.normalized * trait.locomotiveSpeed;
    playerController.Move(velocity);
  }
}
