using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    [SerializeField] AudioSource audioSource;
    void Awake() {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    public void PlaySoundOne() {
        audioSource.Play();
    }
    public void StopSound() {
        audioSource.Stop();
    }
    public void FadeOutSound() {
        StartCoroutine(FadeOutSoundCoroutine());
    }
    public void FadeInSound(float destVolume) {
        StartCoroutine(FadeInSoundCoroutine(destVolume));
    }
    private IEnumerator FadeOutSoundCoroutine() {
        float volume = audioSource.volume;
        float offset = 1;
        while(offset > 0) {
            offset -= Time.deltaTime;
            audioSource.volume = volume * offset;
            yield return null;
        }
    }
    private IEnumerator FadeInSoundCoroutine(float destVolume) {
        float volume = audioSource.volume;
        float offset = 1;
        while(offset > 0) {
            offset -= Time.deltaTime;
            audioSource.volume = Mathf.Lerp(destVolume, volume, offset);
            yield return null;
        }

    }
}