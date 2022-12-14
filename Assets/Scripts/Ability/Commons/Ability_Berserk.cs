using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_Berserk : Ability {
    float[] attackSpeed = { 0.20f, 0.30f, 0.40f, 0.50f, 0.60f };
    bool isActiving = false;
    public override int maxLevel {
        get { return 5; }
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.basicAttackDamageEvent += IncreaseAttackSpeed;
        Player.playerInstance.jumpAttackDamageEvent += IncreaseAttackSpeed;
        Player.playerInstance.attackSpeedCoefs += GetAttackSpeed;
    }
    public override void OnReleaseAbility() {
        Player.playerInstance.basicAttackDamageEvent -= IncreaseAttackSpeed;
        Player.playerInstance.jumpAttackDamageEvent -= IncreaseAttackSpeed;
        Player.playerInstance.attackSpeedCoefs -= GetAttackSpeed;
    }
    Coroutine removeCoroutine;
    public void IncreaseAttackSpeed(Transform target) {
        if(Random.Range(0, 1f) < .15f) {
            isActiving = true;

            if(removeCoroutine != null)
                StopCoroutine(removeCoroutine);

            removeCoroutine = StartCoroutine(Utility.TimeoutTask(() => {
                isActiving = false;
            }, 4f));
        }
    }
    private float GetAttackSpeed() {
        return isActiving ? attackSpeed[level] : 0;
    }
}