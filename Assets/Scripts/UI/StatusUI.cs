using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusUI : UI {
    static public StatusUI instance;

    [SerializeField] Slider rateGaugeSlider;
    [SerializeField] TextMeshProUGUI rateTmp;

    public override bool isActive {
        get {
            return currentFrame.activeInHierarchy;
        }
    }

    protected override void Awake() {
        if(instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public override void KeyPressEvent() {
        base.KeyPressEvent();
        if(Input.GetKeyDown(KeyCode.Tab)) {
            UIManager.instance.CloseUI(this);
        }
    }
    public override void OnActive() {
        currentFrame.SetActive(true);
        activeEvent.Invoke();
    }
    public override void OnInactive() {
        currentFrame.SetActive(false);
        inactiveEvent.Invoke();
    }
    public void UpdateRateUI() {
        rateGaugeSlider.value = 1f * Player.playerInstance.rateGauge / Player.playerInstance.nextRateGauge;
        rateTmp.text = "Rate " + Player.playerInstance.rate;
    }
}