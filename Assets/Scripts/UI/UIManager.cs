using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    static public UIManager instance { get; private set; }
    public UI activingUI { get; private set; } = null;

    public UI escapeMenu;
    [SerializeField] public UI playerStatusUI;

    public void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        if(playerStatusUI == null) {
            Debug.LogWarning("There is any 'Player Status UI'.");
        }
    }
    public void CloseUI() {
        if(activingUI != null) activingUI.OnInactive();
    }
    public void CloseUI(UI targetUI) {
        if(activingUI != null) activingUI.OnInactive();
        activingUI = targetUI;
    }
    public void OpenUI(UI targetUI) {
        if(activingUI != null && activingUI.enabled) {
            activingUI.enabled = false;
            activingUI.OnInactive();
        }
        activingUI = targetUI;
        activingUI.OnActive();
    }
    public void TogglePlayerStatusUI() {
        if(!playerStatusUI.gameObject.activeInHierarchy) {
            OpenUI(playerStatusUI);
        } else {
            CloseUI(playerStatusUI);
        }
    }
}
