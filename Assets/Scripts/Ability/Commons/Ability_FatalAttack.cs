using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_FatalAttack : Ability {
    Player player;
    protected void Start() {
        player = Player.playerInstance;
    }
    public override int maxLevel {
        get { return 3; }
    } // 최대 3레벨
    float[] criticalChance = {.10f, .15f, .20f};
    // 치명타 확률 상승량 10% / 15% / 20%

    public override void OnGetAbility() {
        base.OnGetAbility();
        player.criticalChanceCoef += ReturnCofficient;
    }
    public override void OnReleaseAbility() {
        player.criticalChanceCoef -= ReturnCofficient;
    }
    public float ReturnCofficient() {
        return criticalChance[level];
    }
}