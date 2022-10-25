using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player {
    [Header("Basic Attack Range")]
    [SerializeField] Transform attackRangeCenter;
    [SerializeField] Vector2 attackRangeBounds;

    float attackDamage = 40f;

    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
    }
    protected override void Update() {
        base.Update();
    }
    protected override void InitialState() {
        base.InitialState();
        #region Attack State
        attackState.OnActive += (nextState) => {
            playerAnimator.SetBool("Melee Attack", true);
        };
        attackState.OnInactive += (prevState) => {
            playerAnimator.SetBool("Melee Attack", false);
        };
        #endregion Attack State
    }
    public override void BasicAttack() {
        base.BasicAttack();
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(attackState))
            return;
        if(basicAttackCoroutine != null) StopCoroutine(basicAttackCoroutine);
        basicAttackCoroutine = StartCoroutine(BasicAttackCoroutine());
    }
    public IEnumerator BasicAttackCoroutine() {
        playerStateMachine.ChangeState(attackState);
        yield return new WaitForSeconds(.3f);
        playerStateMachine.ChangeState(basicState);
    }
    void AttackAnimationEventHandler() {
        Collider2D[] inners = Physics2D.OverlapBoxAll(attackRangeCenter.position, attackRangeBounds, 0, Monster.DEFALUT_MONSTER_LAYER);
        foreach(Collider2D inner in inners) {
            IDamageable target;
            if(inner.TryGetComponent<IDamageable>(out target)) {
                Vector2 force = (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up) * 500;
                target.OnDamage(attackDamage, force, .5f);
            }
        }
    }
    void OnDrawGizmos() {
        if(attackRangeCenter != null
        && attackRangeBounds != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(attackRangeCenter.position, attackRangeBounds);
        }
    }
}