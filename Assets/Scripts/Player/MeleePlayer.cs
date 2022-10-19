using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player {
    bool isAttackState;

    [SerializeField] BoxCollider2D meleeAttackRange;

    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
    }
    protected override void InitialState() {
        base.InitialState();
        #region Attack State
        attackState.OnActive += () => {
            playerAnimator.SetBool("Melee Attack 01", true);
        };
        attackState.OnInactive += () => {
            playerAnimator.SetBool("Melee Attack 01", false);
        };
        #endregion Attack State
    }
    public override void BasicAttack() {
        base.BasicAttack();
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(attackState))
            return;
        playerStateMachine.ChangeState(attackState);
        StartCoroutine(CTemporary());
    }
    public IEnumerator CTemporary() {
        yield return new WaitForSeconds(.3f);
        playerStateMachine.ChangeState(basicState);
    }
}