using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class DialogComponent : MonoBehaviour {
    [SerializeField] PlayableDirector director;
    [SerializeField] DialogSet dialogs;

    [SerializeField] TextMeshProUGUI speackerTmp;
    [SerializeField] TextMeshProUGUI scriptTmp;

    public void Awake() {
        director = director==null ? GetComponent<PlayableDirector>() : director;
        dialogs = dialogs==null ? GetComponent<DialogSet>() : dialogs;
    }
    public void PlayTimeline() {
        director.Play();
    }
    public void WaitNext() {
        if(dialogs.Next()) {
            DialogSet.Dialog script = dialogs.GetCurrent();
            speackerTmp.text = script.speacker;
            scriptTmp.text = script.script;
        }
        director.Pause();
    }
    public void CallNext() {
        if(director.state == PlayState.Paused) {
            director.Play();
        }
    }
    public void ChangDirector(PlayableDirector next) {
        director = next;
    }
    public void ClearDialog() {
        speackerTmp.text = "";
        scriptTmp.text = "";
    }
}
