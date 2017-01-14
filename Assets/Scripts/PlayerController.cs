using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController: MonoBehaviour {
  Rigidbody rigidBody;
  Vector3 velocity;
  Map[] maps;

  void Awake() {
    maps = FindObjectOfType<MapGenerator>().maps;
    FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
  }

  void Start() {
    rigidBody = GetComponent<Rigidbody>();
  }

  void FixedUpdate() {
    rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
  }

  public void Move(Vector3 aVelocity) {
    velocity = aVelocity;
  }

  public void LookAt(Vector3 point) {
    // The incoming point is not relative to the player's height. The line below corrects the point
    // so that it is at eye-level with the player.
    Vector3 correctedPoint = new Vector3(point.x, transform.position.y, point.z);
    transform.LookAt(correctedPoint);
  }

  public void OnNewWave(Wave wave, int index) {
    transform.position = maps[index].centerPosition + Vector3.up * 3;
  }
}
