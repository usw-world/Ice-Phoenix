using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_GetRateGauge : Ability
{
    public override int maxLevel {
        get { return 0; }
    }

    public override void OnGetAbility() {
        Player.playerInstance.IncreaseRateGauge(100);
        AbilityManager.instance.RemoveAbility(this);
    }

    public override void OnReleaseAbility() {}
}