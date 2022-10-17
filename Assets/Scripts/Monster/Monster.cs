using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity, IDamageable {
    [SerializeField] protected Animator MonsterAnim;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float detectingDistance;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected Animator monsterAnimator;

    protected override void Awake() {
        base.Awake();
        monsterAnimator = GetComponent<Animator>();
    }

    public void OnDamage(float damage, float duration=.25f) {}
    public void OnDamage(float damage, Vector2 force, float duration=.25f) {}

    protected override void Die() {
        
    }
}
