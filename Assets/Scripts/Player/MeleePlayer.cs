using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player {
    [Header("Basic Attack Range")]
    [SerializeField] int DEBUG_INDEX_ATTACKING;
    [SerializeField] bool DEBUG_JUMP_ATTACK_RANGE;
    [SerializeField] Transform[] attackRange;
    [SerializeField] Transform jumpAttackRange;

    #region Melee Attack
    protected State attackState01 = new State("Attack 01", ATTACK_STATE_TAG);
    protected State attackState02 = new State("Attack 02", ATTACK_STATE_TAG);
    protected State attackState03 = new State("Attack 03", ATTACK_STATE_TAG);
    bool attackingPreInput = false;
    int comboCount = 0;
    int maxComboCount = 3;
    #endregion Melee Attack

    #region Melee Jump Attack
    protected State jumpAttackState = new State("Jump Attack", JUMP_ATTACK_STATE_TAG);
    #endregion Melee Jump Attack

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
        attackState01.OnActive += (prevState) => {
            playerAnimator.SetBool("Melee Attack 01", true);
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
        };
        attackState01.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Melee Attack 01", false);
            if(!nextState.Compare(ATTACK_STATE_TAG)) {
                CommonAttackReleaseEvent();
            }
        };
        attackState02.OnActive += (prevState) => {
            playerAnimator.SetBool("Melee Attack 02", true);
        };
        attackState02.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Melee Attack 02", false);
            if(!nextState.Compare(ATTACK_STATE_TAG)) {
                CommonAttackReleaseEvent();
            }
        };
        attackState03.OnActive += (prevState) => {
            playerAnimator.SetBool("Melee Attack 03", true);
        };
        attackState03.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Melee Attack 03", false);
            if(!nextState.Compare(ATTACK_STATE_TAG)) {
                CommonAttackReleaseEvent();
            }
        };
        void CommonAttackReleaseEvent() {
            playerAnimator.SetTrigger("End Attack");
            isAfterAttack = false;
            attackingPreInput = false;
            comboCount = 0;
            canMove = true;
        }
        #endregion Attack State

        #region Jump Attack State
        jumpAttackState.OnActive += (prevState) => {
            playerAnimator.SetBool("Jump Attack", true);
        };
        jumpAttackState.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Jump Attack", false);
        };
        jumpAttackState.OnStay += () => {
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .05f);
            playerRigidbody.velocity = addingSpeed;
        };
        #endregion Jump Attack State
    }
    public override void Attack() {
        base.Attack();
        if(playerStateMachine.Compare(floatState)) { // Jump Attack Case >>
            JumpAttack();
            return;
        }                                            // << Jumn Attack Case
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG)
        || comboCount >= maxComboCount)
            return;

        if(playerStateMachine.Compare(ATTACK_STATE_TAG) && !isAfterAttack) {
            attackingPreInput = true;
            return;
        }
        AttackProcedure();
    }
    private void AttackProcedure() {
        comboCount++;
        isAfterAttack = false;
        attackingPreInput = false;
        canMove = false;
        switch(comboCount) {
            case 1:
                playerStateMachine.ChangeState(attackState01);
                break;
            case 2:
                playerStateMachine.ChangeState(attackState02);
                break;
            case 3:
                playerStateMachine.ChangeState(attackState03);
                break;
        }
    }
    void AnimationEvent_Attack(int index) {
        Collider2D[] inners = Physics2D.OverlapBoxAll(attackRange[index].position, attackRange[index].localScale, 0, Monster.DEFALUT_MONSTER_LAYER);
        float damage = attackDamage;
        float force = attackForce;
        Vector2 slideforce = new Vector2(transform.localScale.x + moveDirection.x, 0);
        switch(index) {
            case 0:
                damage *= 1f;
                force *= 2f;
                slideforce *= 4;
                break;
            case 1:
                damage *= 1.2f;
                force *= 2f;
                slideforce *= 4;
                break;
            case 2:
                damage *= 1.5f;
                force *= 5f;
                slideforce *= 4;
                break;
        }
        playerRigidbody.AddForce(slideforce, ForceMode2D.Impulse);
        foreach(Collider2D inner in inners) {
            IDamageable target;
            if(inner.TryGetComponent<IDamageable>(out target)) {
                target.OnDamage(
                    attackDamage,
                    (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up*.5f) * force,
                    .5f);
            }
        }
    }
    void AnimationEvent_AfterAttack() {
        if(attackingPreInput) {
            AttackProcedure();
        } else {
            isAfterAttack = true;
        }
    }
    void AnimationEvent_CanMove() {
        canMove = true;
    }
    void AnimationEvent_EndAttack() {
        playerStateMachine.ChangeState(basicState);
        playerAnimator.SetTrigger("End Attack");
    }
    public override void JumpAttack() {
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(jumpAttackState))
            return;
        playerStateMachine.ChangeState(jumpAttackState);
    }
    void AnimationEvent_JumpAttack() {
        Collider2D[] inners = Physics2D.OverlapBoxAll(jumpAttackRange.position, jumpAttackRange.localScale, 0, Monster.DEFALUT_MONSTER_LAYER);
        float damage = attackDamage * .8f;
        float force = attackForce * .6f;
        foreach(Collider2D inner in inners) {
            IDamageable target;
            if(inner.TryGetComponent<IDamageable>(out target)) {
                target.OnDamage(
                    attackDamage,
                    (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up*.5f) * force,
                    .5f);
            }
        }
    }
    void OnDrawGizmos() {
        if(DEBUG_INDEX_ATTACKING >= 0) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(attackRange[DEBUG_INDEX_ATTACKING].position, attackRange[DEBUG_INDEX_ATTACKING].localScale);
        }
        if(DEBUG_JUMP_ATTACK_RANGE) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(jumpAttackRange.position, jumpAttackRange.localScale);
        }
    }
}