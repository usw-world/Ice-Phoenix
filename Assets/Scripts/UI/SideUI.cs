using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideUI : UI {
    [SerializeField] Slider hpSlider;
    void Awake() {
        if(hpSlider == null) Debug.LogWarning($"Hp Slider of {this.gameObject.name} is null.");
    }
    public void SetHP(float value) {
        if(hpSlider != null)
            hpSlider.value = value;
    }
}
