using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AbilitySystem;

public class AbilityIcon : MonoBehaviour {
    [SerializeField] Image abilityImage;
    [SerializeField] TextMeshProUGUI abilityShowingStatus;

    public void SetView(Ability ability) {
        abilityImage.sprite = ability.GetAbilityImage;
    }
}
