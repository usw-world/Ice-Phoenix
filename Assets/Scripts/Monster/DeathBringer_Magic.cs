using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringer_Magic : MonoBehaviourIF {
    DeathBringer owner;
    float damage = 20;
    [SerializeField] Range magicRange;
    public System.Action endEvent;

    public void AnimationEvent_MagicDamage() {
        Collider2D inner = Physics2D.OverlapBox((Vector2)transform.position + magicRange.center, magicRange.bounds, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        if(inner != null) {
            IDamageable target = inner.GetComponent<IDamageable>();
            if(target != null) {
                Vector2 force = (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up) * 300f;
                inner.GetComponent<Player>().OnDamage(damage, force, .25f);
            }
        }
    }
    public void AnimationEvent_MagicEnd() {
        if(endEvent != null)
            endEvent();
    }
    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + magicRange.center, magicRange.bounds);
    }
}
