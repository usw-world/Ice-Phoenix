using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_FireRing : Ability {
    Player player;
    protected void Start() {
        player = Player.playerInstance;
    }
    public override int maxLevel {
        get { return 5; }
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        
    }
    public override void OnReleaseAbility() {
    }
}