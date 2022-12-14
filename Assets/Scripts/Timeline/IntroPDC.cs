using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPDC : MonoBehaviour {
    [SerializeField] UnityEngine.Playables.PlayableDirector director;
    [SerializeField] DialogSet dialogs;

    public void Awake() {
        director = director==null ? GetComponent<UnityEngine.Playables.PlayableDirector>() : director;
    }
    public void StartDialog() {
        director.Pause();
        UIManager.instance.dialogUI.ReadAll(dialogs, () => {
            SetDialogInputState(false);
        });
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
