using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_Feast : Ability {
    float[] amount = { 1f, 1.5f, 2f };
    public override int maxLevel {
        get { return 3; }
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.onDefeatMonster += IncreaseHP;
    }
    public override void OnReleaseAbility() {
        Player.playerInstance.onDefeatMonster -= IncreaseHP;
    }
    public void IncreaseHP(Monster monster) {
        Player.playerInstance.IncreaseMaxHP(amount[level]);
    }
}