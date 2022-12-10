using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilitySystem;
using TMPro;

public class AbilitySummary : MonoBehaviour {
    [SerializeField] Image abilityImage;
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilitySummary;
    [SerializeField] TextMeshProUGUI abilityLevel;

    public void SetView(Ability ability) {
        abilityImage.sprite = ability.GetAbilityImage;
        abilityName.text = ability.GetAbilityName;
        abilitySummary.text = ability.GetSummary;
        abilityLevel.text = ability.GetLevel+1+"";
    }
}
