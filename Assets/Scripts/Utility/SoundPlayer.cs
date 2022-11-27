using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    [SerializeField] AudioSource audioSource;
    public void PlaySoundOne() {
        audioSource.Play();
    }
}