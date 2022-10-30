using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity, IDamageable {
    static public int DEFALUT_MONSTER_LAYER = 128;

    [SerializeField] protected SideUI monsterSideUI;

    [SerializeField] protected Animator MonsterAnim;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float detectingDistance;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected Animator monsterAnimator;

    protected override void Awake() {
        base.Awake();
        monsterAnimator = GetComponent<Animator>();
        if(monsterSideUI != null) {
            monsterSideUI.SetHP(hp / maxHp);
        } else {
            Debug.LogWarning($"There is any Side UI in {this.gameObject.name}");
        }
    }
    protected virtual void Update() {}

    public virtual void OnDamage(float damage, float duration=.25f) {
        hp -= damage;
        if(monsterSideUI != null)
            monsterSideUI.SetHP(hp / maxHp);
    }
    public virtual void OnDamage(float damage, Vector2 force, float duration=.25f) {
        OnDamage(damage, duration);
    }

    protected override void Die() {
        base.Die();
    }
}
