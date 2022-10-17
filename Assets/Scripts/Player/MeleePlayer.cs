using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player {
    bool isAttackState;
    protected State attackState = new State("Attack");

    [SerializeField]
    BoxCollider2D meleeAttackRange;

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
            playerAnimator.SetTrigger("MeleeAttack");
        };
        #endregion Attack State
    }
    public override void BasicAttack()
    {
        base.BasicAttack();

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Weapon_MeleeAttack")) // ������ �� ��� ���� ������ Ȱ��ȭ
            meleeAttackRange.enabled = false;
        else
            meleeAttackRange.enabled = true;
    }
}