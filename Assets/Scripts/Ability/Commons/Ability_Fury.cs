using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_Fury : Ability {
    Player player;
    protected void Start() {
        player = Player.playerInstance;
    }
    public override int maxLevel {
        get { return 3; }
    } // 최대 3레벨
    float[] increasingPercents = {.15f, .40f, .65f};
    // 공격력 상승량 15% / 40% / 65%

    public override void OnGetAbility() {
        base.OnGetAbility();
        player.damageCoefs += ReturnCofficient;
    }
    public override void OnReleaseAbility() {
        player.damageCoefs -= ReturnCofficient;
    }
    public float ReturnCofficient() {
        return Mathf.Lerp(0, increasingPercents[level], (0.8f - (player.hpRatio - 0.2f)) / 0.8f);
    }
}