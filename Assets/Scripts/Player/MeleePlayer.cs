using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Pool;

public class MeleePlayer : Player {
    [Header("Basic Attack Range")]
    [SerializeField] int DEBUG_INDEX_ATTACKING;
    [SerializeField] bool DEBUG_JUMP_ATTACK_RANGE;
    [SerializeField] Range[] attackRange;
    [SerializeField] Range jumpAttackRange;

    #region Melee Attack
    protected State attackState01 = new State("Attack 01", ATTACK_STATE_TAG);
    protected State attackState02 = new State("Attack 02", ATTACK_STATE_TAG);
    protected State attackState03 = new State("Attack 03", ATTACK_STATE_TAG);
    bool attackingPreInput = false;
    int comboCount = 0;
    int maxComboCount = 3;
    [SerializeField] GameObject slashParticle;
    ParticlePool slashParticlePool;
    #endregion Melee Attack
    #region Melee Jump Attack
    protected State jumpAttackState = new State("Jump Attack", JUMP_ATTACK_STATE_TAG);
    #endregion Melee Jump Attack
    #region Sound
    [Header("Additional Sound Clips")]
    [SerializeField] AudioClip[] playerAttackClips;
    [SerializeField] AudioClip[] playerAttackClips_empty;
    [SerializeField] AudioClip playerJumpAttackClip_empty;
    #endregion Sound

    protected override void Awake() {
        base.Awake();
    }
    protected override void Start() {
        base.Start();
        slashParticlePool = new ParticlePool("Slash Particle", slashParticle, 20, 5, transform);
    }
    protected override void Update() {
        base.Update();
    }
    protected override void InitializeState() {
        base.InitializeState();
        #region Attack State
        attackState01.OnActive += (prevState) => {
            playerAnimator.SetBool("Melee Attack 01", true);
            CommonAttackEnterEvent();
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
            CommonAttackEnterEvent();
        };
        attackState02.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Melee Attack 02", false);
            if(!nextState.Compare(ATTACK_STATE_TAG)) {
                CommonAttackReleaseEvent();
            }
        };
        attackState03.OnActive += (prevState) => {
            playerAnimator.SetBool("Melee Attack 03", true);
            CommonAttackEnterEvent();
        };
        attackState03.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Melee Attack 03", false);
            if(!nextState.Compare(ATTACK_STATE_TAG)) {
                CommonAttackReleaseEvent();
            }
        };
        void CommonAttackEnterEvent() {
            playerAnimator.speed = attackSpeed;
        }
        void CommonAttackReleaseEvent() {
            playerAnimator.SetTrigger("End Attack");
            playerAnimator.speed = 1;
            isAfterAttack = false;
            attackingPreInput = false;
            comboCount = 0;
            canMove = true;
        }
        #endregion Attack State

        #region Jump Attack State
        jumpAttackState.OnActive += (prevState) => {
            playerAnimator.SetBool("Jump Attack", true);
            // playerAnimator.SetTrigger("Jump Attack Trigger");
            playerAnimator.speed = attackSpeed;
        };
        jumpAttackState.OnInactive += (State nextState) => {
            playerAnimator.SetBool("Jump Attack", false);
            playerAnimator.speed = 1;
        };
        jumpAttackState.OnStay += () => {
            if(!CheckFront()) return;
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
        Vector2 center = (Vector2)transform.position + attackRange[index].center * transform.localScale;
        Collider2D[] inners = Physics2D.OverlapBoxAll(center, attackRange[index].bounds, 0, Monster.DEFALUT_MONSTER_LAYER);
        float damage = attackDamage;
              damage *= Random.Range(0, 1f)<criticalChance ? 2 : 1;
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
        bool hasTarget = false;
        foreach(Collider2D inner in inners) {
            IDamageable target;
            if(CheckWallBetween(inner.transform, attackRange[index].center.y)) continue;
            hasTarget = true;
            if(inner.TryGetComponent<IDamageable>(out target)) {
                target.OnDamage(
                    damage,
                    (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up*.5f) * force,
                    Color.white,
                    .5f
                );
                if(basicAttackDamageEvent != null)
                    basicAttackDamageEvent(inner.transform);

                GameObject slashEffect = slashParticlePool.OutPool(inner.ClosestPoint((Vector2)transform.position + attackRange[index].center));
                StartCoroutine(Utility.TimeoutTask(() => {
                    slashParticlePool.InPool(slashEffect);
                }, 2f));
            }
        }
        if(hasTarget) {
            int soundIndex = Random.Range(0, playerAttackClips.Length);
            playerSoundPlayer.PlayClip(playerAttackClips[soundIndex]);
        } else {
            playerSoundPlayer.PlayClip(playerJumpAttackClip_empty);
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
        || playerStateMachine.Compare(jumpAttackState)
        || Input.GetButtonDown("Jump"))
            return;
        playerStateMachine.ChangeState(jumpAttackState);
    }
    void AnimationEvent_JumpAttack() {
        float damage = attackDamage * .8f;
              damage *= Random.Range(0, 1f)<criticalChance ? 2 : 1;
        float force = attackForce * .6f;
        Vector2 center = (Vector2)transform.position + jumpAttackRange.center * transform.localScale;
        Collider2D[] inners = Physics2D.OverlapBoxAll(center, jumpAttackRange.bounds, 0, Monster.DEFALUT_MONSTER_LAYER);
        bool hasTarget = false;
        foreach(Collider2D inner in inners) {
            IDamageable target;
            if(CheckWallBetween(inner.transform, jumpAttackRange.center.y)) continue;
            hasTarget = true;
            if(inner.TryGetComponent<IDamageable>(out target)) {
                target.OnDamage(
                    damage,
                    (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up*.5f) * force,
                    Color.white,
                    .5f);
                if(jumpAttackDamageEvent != null)
                    jumpAttackDamageEvent(inner.transform);
            }
            
            GameObject slashEffect = slashParticlePool.OutPool(inner.ClosestPoint((Vector2)transform.position + jumpAttackRange.center));
            StartCoroutine(Utility.TimeoutTask(() => {
                slashParticlePool.InPool(slashEffect);
            }, 2f));
        }
        if(hasTarget) {
            int soundIndex = Random.Range(0, playerAttackClips.Length);
            playerSoundPlayer.PlayClip(playerAttackClips[soundIndex]);
        } else {
            playerSoundPlayer.PlayClip(playerJumpAttackClip_empty);
        }
    }
    private bool CheckWallBetween(Transform target, float pointY) {
        Collider2D collider;
        if(target.TryGetComponent<Collider2D>(out collider)) {
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)transform.position + new Vector2(0, pointY),
                new Vector2(transform.localScale.x, 0),
                Vector2.Distance((Vector2)transform.position, target.position) - target.GetComponent<Collider2D>().bounds.size.x/2,
                1<<6
            );
            return !!hit && hit.collider.tag == "Ground";
        }
        return false;
    }
    void OnDrawGizmos() {
        if(DEBUG_INDEX_ATTACKING >= 0) {
            Gizmos.color = Color.green;
            Vector2 center = (Vector2)transform.position + attackRange[DEBUG_INDEX_ATTACKING].center * transform.localScale;
            Gizmos.DrawWireCube(center, attackRange[DEBUG_INDEX_ATTACKING].bounds);
        }
        if(DEBUG_JUMP_ATTACK_RANGE) {
            Gizmos.color = Color.green;
            Vector2 center = (Vector2)transform.position + jumpAttackRange.center * transform.localScale;
            Gizmos.DrawWireCube(center, jumpAttackRange.bounds);
        }
    }
}