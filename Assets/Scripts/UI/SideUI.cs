using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideUI : UI {
    [SerializeField] Slider hpSlider;
    protected override void Awake() {
        base.Awake();

        if(hpSlider == null) Debug.LogWarning($"Hp Slider of {this.gameObject.name} is null.");
    }
    public void UpdateHPSlider(LivingEntity target) {
        if(hpSlider != null)
            hpSlider.value = target.hpRatio;
    }
}
