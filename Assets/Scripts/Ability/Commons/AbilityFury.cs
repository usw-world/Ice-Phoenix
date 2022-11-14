using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class AbilityFury : Ability {
    [Range(0f, 1f)]
    float weight = 0f;
    
    protected override int maxLevel {
        get { return 3; }
    }
    float[] increasingPercents = {15, 40, 65};

    public override void OnGetAbility() {
        
    }
    protected override void Start() {
        base.Start();
    }
}