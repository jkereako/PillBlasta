using UnityEngine;
using System.Collections;

public class AudioManager: MonoBehaviour {
  public static AudioManager instance;

  const float masterVolumePercent = 0.2f;
  const float fxVolumePercent = 1.0f;
  const float musicVolumePercent = 1.0f;
  AudioSource previouslyActiveMusicSource;
  AudioSource activeMusicSource;
  AudioSource[] musicSources;

  void Awake() {
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(this.gameObject);

      musicSources = new AudioSource[2];

      for (int i = 0; i < musicSources.Length; i++) {
        GameObject musicSource = new GameObject("MusicSource" + i);
        musicSources[i] = musicSource.AddComponent<AudioSource>();
        musicSource.transform.parent = transform;
      }

      activeMusicSource = musicSources[0];
    }
    else {
      Debug.Log("Destroy Audio Manager");
      Destroy(this);
    }
  }

  public void PlaySoundEffect(AudioClip clip, Vector3 position) {
    AudioSource.PlayClipAtPoint(clip, position, fxVolumePercent * masterVolumePercent);
  }

  public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f) {
    previouslyActiveMusicSource = activeMusicSource;
    activeMusicSource.clip = clip;
    activeMusicSource.Play();

    StartCoroutine(CrossFade(fadeDuration));
  }

  IEnumerator CrossFade(float duration) {
    float percentCompleted = 0;

    while (percentCompleted < 1) {
      percentCompleted += Time.deltaTime * 1 / duration;
      previouslyActiveMusicSource.volume = Mathf.Lerp(
        musicVolumePercent * masterVolumePercent, 0, percentCompleted
      );
      activeMusicSource.volume = Mathf.Lerp(
        0, musicVolumePercent * masterVolumePercent, percentCompleted
      );

      yield return null;
    }
  }
}
