using UnityEngine;

public class MusicManager: MonoBehaviour {

  public AudioClip mainTheme;
  public AudioClip menuTheme;

  void Start() {
    Debug.Log("Music Manager");

    if (AudioManager.instance == null) {
      Debug.Log("Audio Manager is null");
    }
 
    AudioManager.instance.PlayMusic(menuTheme, 2);
  }

  void Update() {
  }
}
