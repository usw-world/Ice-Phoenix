using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : LivingEntity, IDamageable {
    static public int DEFALUT_MONSTER_LAYER = 128;

    [SerializeField] protected SideUI monsterSideUI;
    
    [SerializeField] protected Animator monsterAnimator;
    [SerializeField] protected Rigidbody2D monsterRigidbody;

    protected override void Awake() {
        base.Awake();
        monsterAnimator = GetComponent<Animator>();
        if(monsterAnimator == null)
            Debug.LogWarning($"Monster hasn't any 'Animator'.");

        if(monsterSideUI != null)
            monsterSideUI.UpdateHPSlider(this);
        else
            Debug.LogWarning("Monster hasn't any 'Side UI'.");

        if (monsterRigidbody==null && !TryGetComponent<Rigidbody2D>(out monsterRigidbody))
            Debug.LogWarning("Monster hasn't any 'Rigidbody2D'.");
    }
    protected virtual void Update() {}

    private float IncreasHP(float amount) {
        float nextHp = SetHP(hp + amount);
        return hp;
    }
    public virtual void OnDamage(float damage, float duration=.25f) {
        IncreasHP(-damage);
        if(monsterSideUI != null)
            monsterSideUI.UpdateHPSlider(this);
    }
    public virtual void OnDamage(float damage, Vector2 force, float duration=.25f) {
        OnDamage(damage, duration);
    }

    protected override void Die() {
        base.Die();
    }
}
