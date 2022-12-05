using UnityEngine;

public class StatusUI : UI {
    public override void KeyPressEvent() {
        base.KeyPressEvent();
        if(Input.GetKeyDown(KeyCode.Tab)) {
            UIManager.instance.CloseUI(this);
        }
    }
}