using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

public class Ability_FireWalk : Ability
{
    public override int maxLevel => 3;

    private float[] speed = { .2f, .4f, .6f };
    private float[] armor = { -.15f, -.2f, -.25f };

    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.armorCoefficients += DecreaseArmor;
        Player.playerInstance.moveSpeedAttribute += IncreaseSpeed;
    }

    public override void OnReleaseAbility() {
        Player.playerInstance.armorCoefficients -= DecreaseArmor;
        Player.playerInstance.moveSpeedAttribute -= IncreaseSpeed;
    }
    public float IncreaseSpeed() { return speed[level]; }
    public float DecreaseArmor() { return armor[level]; }
}
