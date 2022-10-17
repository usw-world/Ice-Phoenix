using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSideUI : UI {
    [SerializeField] Slider hpSlider;
    void Awake() {
        if(hpSlider == null) Debug.LogWarning($"Hp Slider is null.");
    }
    public void SetHPSlider(float value) {
        if(hpSlider != null)
            hpSlider.value = value;
    }
}
