using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPDC : MonoBehaviour {
    GameObject activingObject;
    [SerializeField] BossMonster boss;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioClip battlePhaseBGMClip;

    public void ChangeInputStateDialog() {
        InputManager.instance.ChangeToDialogState();
    }
    public void ChangeInputStatePlay() {
        InputManager.instance.ChangeToPlayState();
    }
    public void ActiveBoss() {
        boss.enabled = true;
    }
    public void MusicChange() {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = battlePhaseBGMClip;
        bgmAudioSource.Play();
    }
}
