using UnityEngine;

// Prevent the player from staying in one spot too long.
public struct PlayerCampManager {
  readonly Transform player;
  Vector3 lastPosition;
  const float timeDelay = 2.0f;
  const float distanceThreshold = 1.5f;
  float nextCheckTime;
  bool _isPlayerCamped;

  public bool isPlayerCamped {
    get {
      if (Time.time > nextCheckTime) {
        nextCheckTime = Time.time + timeDelay;
        _isPlayerCamped = Vector3.Distance(player.position, lastPosition) < distanceThreshold;
        lastPosition = player.position;
      }

      return _isPlayerCamped;
    }
  }

  public PlayerCampManager(Transform aPlayer) {
    _isPlayerCamped = false;
    player = aPlayer;
    nextCheckTime = timeDelay + Time.time;
    lastPosition = player.position;
  }
}
