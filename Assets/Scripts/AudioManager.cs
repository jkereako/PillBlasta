using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using UnityEditor;

public class AudioManager: MonoBehaviour {
  float masterVolumePercent;
  float fxVolumePercent;
  float musicVolumePercent;
  AudioSource previouslyActiveMusicSource;
  AudioSource activeMusicSource;
  AudioSource[] musicSources;

  void Awake() {
    AudioSource[] musicSources = new AudioSource[2];

    for (int i = 0; i < musicSources.Length; i++) {
      GameObject musicSource = new GameObject("MusicSource" + i);
      musicSources[i] = musicSource.AddComponent<AudioSource>();
      musicSource.transform.parent = transform;
    }
  }

  void PlaySoundEffect(AudioClip clip, Vector3 position) {
    AudioSource.PlayClipAtPoint(clip, position, fxVolumePercent * masterVolumePercent);
  }

  void PlayMusic(AudioClip clip, float fadeDuration = 1.0f) {
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
