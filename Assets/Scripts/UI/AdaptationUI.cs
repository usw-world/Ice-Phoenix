using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdaptationUI : MonoBehaviour {
    [SerializeField] StatusUI playerStatusUI;
    AdaptationManager adaptation => playerStatusUI.adaptationManager;

    [SerializeField] TextMeshProUGUI[] pointText = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI remainingPointText;

    void Start() {
        RefreshAmount();
    }
    public void RefreshAmount() {
        pointText[(int)AdaptationManager.Type.Power].SetText($"{adaptation.points[(int)AdaptationManager.Type.Power]} / {adaptation.maxPoints[(int)AdaptationManager.Type.Power]}");
        pointText[(int)AdaptationManager.Type.Movement].SetText($"{adaptation.points[(int)AdaptationManager.Type.Movement]} / {adaptation.maxPoints[(int)AdaptationManager.Type.Movement]}");
        pointText[(int)AdaptationManager.Type.Fast].SetText($"{adaptation.points[(int)AdaptationManager.Type.Fast]} / {adaptation.maxPoints[(int)AdaptationManager.Type.Fast]}");
        pointText[(int)AdaptationManager.Type.Strong].SetText($"{adaptation.points[(int)AdaptationManager.Type.Strong]} / {adaptation.maxPoints[(int)AdaptationManager.Type.Strong]}");
        pointText[(int)AdaptationManager.Type.Ability].SetText($"{adaptation.points[(int)AdaptationManager.Type.Ability]} / {adaptation.maxPoints[(int)AdaptationManager.Type.Ability]}");
        remainingPointText.SetText(adaptation.remainingPoint.ToString());
    }
    private void ApplyAdaptation() {
        GameManager.instance.SetAdaptations(adaptation.points);
    }
    public void Increase(int type) {
        try {
            if(Input.GetKey(KeyCode.LeftShift))
                adaptation.Increase((AdaptationManager.Type)type, 5);
            else
                adaptation.Increase((AdaptationManager.Type)type);
        } catch(System.Exception e) {
            print(e.Message);
        }
        RefreshAmount();
    }
    public void Decrease(int type) {
        try {
            if(Input.GetKey(KeyCode.LeftShift))
                adaptation.Decrease((AdaptationManager.Type)type, 5);
            else
                adaptation.Decrease((AdaptationManager.Type)type);
        } catch(System.Exception e) {
            Debug.LogWarning(e.Message);
        }
        RefreshAmount();
    }
    public void OnOpenUI() {
        RefreshAmount();
    }
    public void OnCloseUI() {
        ApplyAdaptation();
    }
}
