using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CmpLinkedList;

public class LivingEntity : MonoBehaviourIF {
    public bool isDead { get; private set; } = false;
    [Header("Living Entity")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp { get; private set; }
    public IntegratedList<Attach> havingAttach { get; protected set; } = new IntegratedList<Attach>();

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
        while(havingAttach.Count > 0) {
            Attach t = havingAttach[0];
            ReleaseAttach(t);
            if(havingAttach.Contains(t)) {
                havingAttach.Remove(t);
            }
        }
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

    public void GetAttach(Attach attach) {
        attach.attachedTarget = this;
        attach.OnAttach();
        havingAttach.Push(attach);
    }
    public void ReleaseAttach(Attach attach) {
        Attach target;
        if(havingAttach.Shift(attach, out target)) {
            target.OnDetach();
            target.attachedTarget = null;
        }
    }
}