using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    UI activingUI = null;
    
    public void CloseUI() {
        activingUI.OnInactive();
    }
    public void OpenUI(UI targetUI) {
        activingUI.OnInactive();
        activingUI = targetUI;
        activingUI.OnActive();
    }
}
