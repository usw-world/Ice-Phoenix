using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_Thunderbolt : Ability
{
    public override int maxLevel {
        get { return 3; }
    }
    float[] thunderboltchance = {.15f, .20f, .25f};
    float[] thunderboltDamage = {30, 40, 50};
    EffectPool thunderboltPool;
    [SerializeField] GameObject thunderboltEffect;

    void Start() {
        thunderboltPool = new EffectPool("Thunderbolt", thunderboltEffect, 5, 3);
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.basicAttackDamageEvent += DropThunder;
        Player.playerInstance.jumpAttackDamageEvent += DropThunder;
    }

    public override void OnReleaseAbility() {
        Player.playerInstance.basicAttackDamageEvent -= DropThunder;
        Player.playerInstance.jumpAttackDamageEvent -= DropThunder;
    }
    private void DropThunder(Transform target) {
        if(Random.Range(0, 1f) < thunderboltchance[level]) {
            GameObject effect = thunderboltPool.OutPool((Vector2)target.transform.position + Vector2.up * 1.4f);
            Thunderbolt thunderbolt;
            if(effect.TryGetComponent<Thunderbolt>(out thunderbolt)) {
                thunderbolt.damage = thunderboltDamage[level];
                thunderbolt.endEvent = () => {
                    thunderboltPool.InPool(effect);
                };
            }
        }
    }
}