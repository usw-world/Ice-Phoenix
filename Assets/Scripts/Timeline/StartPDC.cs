using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPDC : MonoBehaviour {
    [SerializeField] UnityEngine.Playables.PlayableDirector director;
    [SerializeField] DialogSet dialogs;

    public void Awake() {
        director = director==null ? GetComponent<UnityEngine.Playables.PlayableDirector>() : director;
    }
    public void ChangePlayable(UnityEngine.Playables.PlayableAsset playable) {
        director.playableAsset = playable;
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
