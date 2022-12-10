using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AbilitySystem;

public class AbilityChoicesUI : UI {
    [SerializeField] private Button firstCard;
    [System.Serializable]
    private class AbilityChoice {
        public Ability ability;
        [SerializeField] public Image icon;
        [SerializeField] public TextMeshProUGUI abilityName;
        [SerializeField] public TextMeshProUGUI abilityLevel;
        [SerializeField] public TextMeshProUGUI abilityDescription;
        [SerializeField] public TextMeshProUGUI abilityAnnotation;
        [SerializeField] public TextMeshProUGUI abilitySummary;
    }

    public override bool isActive => canvas.activeInHierarchy;

    [SerializeField] private AbilityChoice[] abilityChoices;

    static public bool isChoosing { get; private set; } = false;

    public void ShowChoices() {
        isChoosing = true;
        UIManager.instance.OpenUI(this);
        firstCard.Select();
    }
    public void SetChoice(int index, Ability ability) {
        abilityChoices[index].ability = ability;
        abilityChoices[index].icon.sprite = ability.GetAbilityImage;
        abilityChoices[index].abilityName.text = ability.GetAbilityName;
        abilityChoices[index].abilityLevel.text =  ability.GetLevel + (ability.playerHas?2:1) + "";
        abilityChoices[index].abilityDescription.text = ability.GetDescription;
        abilityChoices[index].abilityAnnotation.text = ability.GetAnnotation;
    }
    public void SelectChoice(int index) {
        if(index > 2) return;
        AbilityManager.instance.AddAbility(abilityChoices[index].ability);
        isChoosing = false;
    }
    public override void KeyPressEvent() {}
}
