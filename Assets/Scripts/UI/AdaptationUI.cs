using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdaptationUI : MonoBehaviour {
    [SerializeField] Adaptation adaptation;
    [SerializeField] GameObject playerObject;

    [SerializeField] TextMeshProUGUI[] pointText = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI remainingPointText;
    
    void Awake() {
        if(playerObject == null)
            playerObject = GameObject.FindWithTag("Player");
        if(playerObject != null)
            adaptation = playerObject.GetComponentInChildren<Adaptation>();
        if(adaptation == null) {
            Destroy(this);
            throw new System.NotImplementedException("AdaptationUI Script hasn't gotten any 'Adaption' Script in children of player gameObject.");
        }
    }
    void Start() {
        RefreshAmount();
    }
    public void RefreshAmount() {
        pointText[(int)Adaptation.Type.Power].SetText($"{adaptation.points[(int)Adaptation.Type.Power]} / {adaptation.maxPoints[(int)Adaptation.Type.Power]}");
        pointText[(int)Adaptation.Type.Movement].SetText($"{adaptation.points[(int)Adaptation.Type.Movement]} / {adaptation.maxPoints[(int)Adaptation.Type.Movement]}");
        pointText[(int)Adaptation.Type.Fast].SetText($"{adaptation.points[(int)Adaptation.Type.Fast]} / {adaptation.maxPoints[(int)Adaptation.Type.Fast]}");
        pointText[(int)Adaptation.Type.Strong].SetText($"{adaptation.points[(int)Adaptation.Type.Strong]} / {adaptation.maxPoints[(int)Adaptation.Type.Strong]}");
        pointText[(int)Adaptation.Type.Ability].SetText($"{adaptation.points[(int)Adaptation.Type.Ability]} / {adaptation.maxPoints[(int)Adaptation.Type.Ability]}");
        remainingPointText.SetText(adaptation.remainingPoint.ToString());
    }

    public void Increase(int type) {
        try {
            if(Input.GetKey(KeyCode.LeftShift))
                adaptation.Increase((Adaptation.Type)type, 5);
            else
                adaptation.Increase((Adaptation.Type)type);
        } catch(System.Exception e) {
            print(e.Message);
        }
        RefreshAmount();
    }
    public void Decrease(int type) {
        try {
            if(Input.GetKey(KeyCode.LeftShift))
                adaptation.Decrease((Adaptation.Type)type, 5);
            else
                adaptation.Decrease((Adaptation.Type)type);
        } catch(System.Exception e) {
            print(e.Message);
        }
        RefreshAmount();
    }
    public void OnOpenUI() {
        RefreshAmount();
    }
}
