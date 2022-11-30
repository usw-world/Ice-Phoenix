using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public abstract class ChaseMonster : Monster {
    [SerializeField] protected StateMachine monsterStateMachine;
    protected State idleState = new State("Idle");
    protected State chaseState = new State("Chase");

    [SerializeField] protected float moveSpeed = 5f;
    protected float remainingDistance = 0;
    [SerializeField] protected float stoppingDistance = 1f;
    [SerializeField] protected Transform targetTransform;

    [SerializeField] protected Area detectRange;
    
    protected int targetDirection {
        get {
            return targetTransform!=null && targetTransform.position.x-transform.position.x>0 ? 1 : -1;
        }
    }

    protected override void Awake() {
        base.Awake();
        if (TryGetComponent<StateMachine>(out monsterStateMachine)) {
            monsterStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Monster hasn't any 'StateMachine'.");
        }
        InitializeState();
    }
    protected virtual void InitializeState() {
        chaseState.OnStay += () => {
            LookAtX(targetDirection);
            if(CanChase()) {
                Vector2 moveSpace = new Vector2(targetDirection, 0) * moveSpeed * Time.deltaTime;
                transform.Translate(moveSpace);
                remainingDistance -= moveSpace.magnitude;
            } else {
                monsterStateMachine.ChangeState(idleState);
            }
        };
    }
    protected override void Start() {
        base.Start();
    }
    protected override void Update() {
        base.Update();
        ChaseTarget();
    }
    protected abstract void DetectTarget();
    protected abstract void MissTarget();
    protected virtual void ChaseTarget() {
        if(!isDead && CanChase()) {
            monsterStateMachine.ChangeState(chaseState, false);
        }
    }
    protected virtual bool IsArrive() {
        return remainingDistance <= stoppingDistance;
    }
    protected virtual bool CanChase() {
        return targetTransform!=null && !IsArrive();
    }

    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + detectRange.center, detectRange.radius);
    }
}