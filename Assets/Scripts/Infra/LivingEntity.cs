using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LivingEntity : MonoBehaviourIF {
    protected bool isDead { get; private set; } = false;
    [Header("Living Entity")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp { get; private set; }
    public float hpRatio {
        get {
            return this.hp / this.maxHp;
        }
    }
    protected virtual void Awake() {}
    protected virtual void OnEnable() {
        hp = maxHp;
        isDead = false;
    }
    protected virtual void Start() {}
    protected virtual void Die() {
        isDead = true;
    }
    protected virtual float SetHP(float next) {
        hp = next;
        return hp;
    }
    protected virtual float SetMaxHP(float next, bool hpFollow=false) {
        float ratio = hp / maxHp;
        maxHp = next;
        if(hpFollow)
            SetHP(maxHp * ratio);
        return maxHp;
    }
}