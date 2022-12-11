using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenUI : UI {
    static public ScreenUI instance;
    [SerializeField] Slider playerHpSlider;
    [SerializeField] AbilityIconList abilityIconList;

    [SerializeField] Slider expSlider;
    [SerializeField] TextMeshProUGUI levelText;
    Coroutine expSmoothIncreaseCoroutine;

    [SerializeField] TextMeshProUGUI stageNameTmp;

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
        levelText.text = Player.playerInstance.playerLevel + ".Lv";
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
    public void UpdateAbilityIcons() {
        abilityIconList.Redraw();
    }
    Coroutine showStageNameCoroutine;
    public void ShowStageName(string stageName) {
        stageNameTmp.text = stageName;
        
        if(showStageNameCoroutine != null)
            StopCoroutine(showStageNameCoroutine);
        showStageNameCoroutine = StartCoroutine(ShowStageNameCoroutine());
    }
    private IEnumerator ShowStageNameCoroutine() {
        Color start = stageNameTmp.color;
        float offset = 0;
        stageNameTmp.gameObject.SetActive(true);
        while(offset < 1) {
            offset += Time.deltaTime;
            stageNameTmp.color = new Color(start.r, start.g, start.b, offset);
            yield return null;
        }
        yield return new WaitForSeconds(2);
        while(offset > 0) {
            offset -= Time.deltaTime;
            stageNameTmp.color = new Color(start.r, start.g, start.b, offset);
            yield return null;
        }
        stageNameTmp.gameObject.SetActive(false);
    }
}