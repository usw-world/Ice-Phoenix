using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LivingEntity : MonoBehaviourIF {
    protected bool isDead { get; private set; } = false;
    [Header("Living Entity")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;

    protected virtual void Awake() {
        hp = maxHp;
    }
    protected virtual void Start() {}
    protected virtual void Die() {
        isDead = true;
    }
}