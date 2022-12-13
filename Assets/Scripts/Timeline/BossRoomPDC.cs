using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPDC : MonoBehaviour {
    GameObject activingObject;
    
    public void ActiveBossRoomParts() {
        activingObject.SetActive(true);
    }
    public void ChangeInputStateDialog() {
        InputManager.instance.ChangeToDialogState();
    }
    public void ChangeInputStatePlay() {
        InputManager.instance.ChangeToPlayState();
    }
}
