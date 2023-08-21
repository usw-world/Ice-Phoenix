using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class StartPDC : MonoBehaviour {
    [SerializeField] GameObject tutorialUI;
    [SerializeField] PlayableDirector director;
    [SerializeField] DialogSet dialogs;

    private void Start() {
        director = director==null ? GetComponent<UnityEngine.Playables.PlayableDirector>() : director;
        
        var timelineAsset = director.playableAsset as TimelineAsset;
        foreach(TrackAsset track in timelineAsset.GetOutputTracks()) {
            switch(track.name) {
                case "Zoom In Camera":
                    director.SetGenericBinding(track, Player.playerInstance.zoomCamera);
                    break;
                case "Zoom Out Camera":
                    director.SetGenericBinding(track, Player.playerInstance.normalCamera);
                    break;
                case "Dialog Border":
                    director.SetGenericBinding(track, UIManager.instance.dialogUI.frame);
                    break;
            }
        }
    }
    public void ChangePlayable(UnityEngine.Playables.PlayableAsset playable) {
        director.playableAsset = playable;

        var timelineAsset = director.playableAsset as TimelineAsset;

        director.Play();
    }
    public void SetDialog(DialogSet dialogs) {
        this.dialogs = dialogs;
    }
    public void StartDialog() {
        UIManager.instance.dialogUI.ReadAll(dialogs, () => {
            SetDialogInputState(false);
        });
        director.Pause();
        // director.Stop();
    }
    public void SetDialogInputState(bool isDialogState) {
        if(isDialogState) {
            InputManager.instance.ChangeToDialogState();
        } else {
            InputManager.instance.ChangeToPlayState();
            director.Play();
        }
    }
}
