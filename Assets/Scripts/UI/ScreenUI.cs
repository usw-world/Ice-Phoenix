using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenUI : UI {
    static public ScreenUI instance;
    [SerializeField] Slider playerHpSlider;

    [SerializeField] Slider expSlider;
    Coroutine expSmoothIncreaseCoroutine;

    [SerializeField] Slider rateGaugeSlider;
    [SerializeField] TextMeshProUGUI rateTmp;

    protected override void Awake() {
        base.Awake();
        if(instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }
    public void UpdateHPSlider(LivingEntity target) {
        if(playerHpSlider != null)
            playerHpSlider.value = target.hpRatio;
    }
    public void UpdateExpSlider() {
        if(expSmoothIncreaseCoroutine != null)
            StopCoroutine(expSmoothIncreaseCoroutine);
        expSmoothIncreaseCoroutine = StartCoroutine(ExpSmoothIncreaseCoroutine());
    }
    public void UpdateRateUI() {
        rateGaugeSlider.value = 1f * Player.playerInstance.rateGauge / Player.playerInstance.nextRateGauge;
        rateTmp.text = Player.playerInstance.rate+"";
    }
    private IEnumerator ExpSmoothIncreaseCoroutine() {
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime/3;
            float next = (float)Player.playerInstance.currentExp / Player.playerInstance.nextLevelExp;
            expSlider.value = Mathf.Lerp(expSlider.value, next, offset);
            yield return null;
        }
    }
}