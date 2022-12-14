using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThunderbolt : MonoBehaviourIF {
    [SerializeField] Range damageRange;
    public float damage = 35;
    public System.Action endEvent;
    public SoundPlayer soundPlayer;

    public void AnimationEvent_SoundPlay() {
        soundPlayer.StopSound();
        soundPlayer.PlaySoundOne();
    }
    public void AnimationEvent_Dropped() {
        if(damage < 0) {
            Debug.LogWarning("The Damage of Thunderbolt is undefineded.");
            return;
        }
        Collider2D inner = Physics2D.OverlapBox((Vector2)transform.position + damageRange.center, damageRange.bounds, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        IDamageable target;
        if(inner && inner.TryGetComponent<IDamageable>(out target)) {
            Vector2 force = (((Vector2)(inner.transform.position - transform.position)) * Vector2.right).normalized * 300f;
            target.OnDamage(damage, force, new Color(.85f, .25f, 1), .75f);
        }
    }
    public void AnimationEvent_ThunderboltEnd() {
        if(endEvent != null)
            endEvent();
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 1, 1);
        Gizmos.DrawWireCube((Vector2)transform.position + damageRange.center, damageRange.bounds);
    }
}
