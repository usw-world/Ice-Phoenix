using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AbilitySystem;

public class StatusUI : UI {
    static public StatusUI instance;

    [SerializeField] private AudioClip openingSound;

    public AdaptationManager adaptationManager {
        get {
            return AdaptationManager.instance;
        }
    }
    public AbilityManager abilityManager {
        get {
            return AbilityManager.instance;
        }
    }

    [SerializeField] Slider rateGaugeSlider;
    [SerializeField] TextMeshProUGUI rateTmp;
    
    [SerializeField] AbilityUI abilityUI;
    [SerializeField] Button[] adaptationButtons;

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

    public override void KeyPressEvent() {
        base.KeyPressEvent();
        if(Input.GetKeyDown(KeyCode.Tab)) {
            UIManager.instance.CloseUI(this);
        }
    }
    public override void OnActive() {
        canvas.SetActive(true);
        activeEvent.Invoke();
        UIManager.instance.soundPlayer.PlayClip(openingSound);
    }
    public override void OnInactive() {
        canvas.SetActive(false);
        inactiveEvent.Invoke();
    }
    public void UpdateRateUI() {
        rateGaugeSlider.value = 1f * Player.playerInstance.rateGauge / Player.playerInstance.nextRateGauge;
        rateTmp.text = "Rate " + Player.playerInstance.rate;
    }
    public void UpdateAbilityUI() {
        abilityUI.Redraw();
    }
    public void SetActiveAdaptationButtons(bool next) {        
        foreach(Button button in adaptationButtons) {
            button.interactable = next;
        }
    }
}