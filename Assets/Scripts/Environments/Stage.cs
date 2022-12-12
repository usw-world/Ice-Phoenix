using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour {
    static public Stage currentStage;
    [SerializeField] string stageName = "";
    [SerializeField] PolygonCollider2D mapConfiner;

    public enum StageType {
        None,
        Intro,
        Safe,
        Unsafe
    }

    void Awake() {
        if(currentStage == null) {
            currentStage = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    [SerializeField] public StageType type = StageType.None;
    [SerializeField] private Transform startPoint;
    void Start() {
        if(mapConfiner != null)
            Player.playerInstance.normalCamera.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = mapConfiner;
        Player.playerInstance.transform.position = startPoint.position;
        UIManager.instance.FadeIn(() => {});
        UIManager.instance.screenUI.ShowStageName(stageName);
        if(type != StageType.Unsafe) {
            UIManager.instance.playerStatusUI.SetActiveAdaptationButtons(true);
        } else {
            UIManager.instance.playerStatusUI.SetActiveAdaptationButtons(false);
        }
    }
}
