using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscapeMenuUI : UI {
    public override bool isActive => canvas.activeInHierarchy;

    public override void OnActive() {
        canvas.SetActive(true);
        activeEvent.Invoke();
        GameManager.instance.StopGame();
    }
    public override void OnInactive() {
        GameManager.instance.StartGame();
        inactiveEvent.Invoke();
        canvas.SetActive(false);
    }
}