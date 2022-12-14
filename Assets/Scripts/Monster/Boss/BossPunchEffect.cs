using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPunchEffect : MonoBehaviourIF {
    [SerializeField] Range damage_range;
    [SerializeField] Range damageRange { get { return new Range((Vector2)transform.position+damage_range.center, damage_range.bounds); } }
    public System.Action endEvent;

    void OnEnable() {
        Collider2D inner = Physics2D.OverlapBox(damageRange.center, damageRange.bounds, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        if(inner) {
            int dir = transform.localScale.x>0 ? -1 : 1;
            inner.GetComponent<IDamageable>().OnDamage(50, new Vector2(dir * 1500, 750), 1f);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damageRange.center, damageRange.bounds);
    }
    public void AnimationEvent_EffectEnd() {
        endEvent();
    }
}
