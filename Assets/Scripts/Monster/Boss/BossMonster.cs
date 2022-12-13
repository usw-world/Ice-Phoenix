using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class BossMonster : Monster {
    const string ATTACK_STATE_TAG = "tag:Attack";

    State moveState = new State("Move");
    State punchState = new State("Punch", ATTACK_STATE_TAG);
    State summonThunderState = new State("Summon Thunder", ATTACK_STATE_TAG);
    State summonDogsState = new State("Summon Dogs", ATTACK_STATE_TAG);
    State summonMagesState = new State("Summon Mages", ATTACK_STATE_TAG);
    State hitState = new State("Hit");

    bool isAngry = false;

    void Move() {

    }
    public override void OnDamage(float damage, float duration=.25f) {
        base.OnDamage(damage, duration);
        
        if(hp < maxHp/2) {
            isAngry = true;
            SpriteRenderer renderer;
            if(TryGetComponent<SpriteRenderer>(out renderer)) {
                renderer.color = new Color(.5f, 0, .15f);
            }
        }
    }
}