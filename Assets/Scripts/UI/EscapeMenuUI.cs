using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscapeMenuUI : UI {
    public override bool isActive => canvas.activeInHierarchy;

    public override void OnActive() {
        canvas.SetActive(true);
        activeEvent.Invoke();
    }
    public override void OnInactive() {
        inactiveEvent.Invoke();
        canvas.SetActive(false);
    }
}