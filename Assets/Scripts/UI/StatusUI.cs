using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AbilitySystem;

public class StatusUI : UI {
    static public StatusUI instance;

    public AdaptationManager adaptationManager;
    public AbilityManager abilityManager;

    [SerializeField] Slider rateGaugeSlider;
    [SerializeField] TextMeshProUGUI rateTmp;

    public override bool isActive {
        get {
            return canvas.activeInHierarchy;
        }
    }

    protected override void Awake() {
        if(instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start() {
        adaptationManager = AdaptationManager.instance;
        abilityManager = AbilityManager.instance;
    }

    public override void KeyPressEvent() {
        base.KeyPressEvent();
        if(Input.GetKeyDown(KeyCode.Tab)) {
            UIManager.instance.CloseUI(this);
        }
    }
    public override void OnActive() {
        canvas.SetActive(true);
        activeEvent.Invoke();
    }
    public override void OnInactive() {
        canvas.SetActive(false);
        inactiveEvent.Invoke();
    }
    public void UpdateRateUI() {
        rateGaugeSlider.value = 1f * Player.playerInstance.rateGauge / Player.playerInstance.nextRateGauge;
        rateTmp.text = "Rate " + Player.playerInstance.rate;
    }
}