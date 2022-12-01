using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class DeathBringer : ChaseMonster {
    const string ATTACK_STATE_TAG = "tag:Attack";

    State attackState = new State("Attack", ATTACK_STATE_TAG);
    State magicState = new State("Magic", ATTACK_STATE_TAG);
    State hitState = new State("Hit");

    private float attackDamage = 15f;
    private float lastAttackTime = 0f;
    private float attackInterval = 1.2f;
    private float attackDistance = 1.5f;

    private float magicDamage = 22f;
    private float lastMagicTime = 0f;
    private float magicInterval = 4f;
    private float magicDistance = 6.5f;
    private Vector2 nextMagicPoint;
    [SerializeField] private GameObject magicInstance;
    private EffectPool magicEffectPool;

    [SerializeField] Range attackArea;
    Range damageArea {
        get {
            return new Range(
                (Vector2)transform.position + attackArea.center * new Vector2(transform.localScale.x, 1),
                attackArea.bounds
            );
        }
    }

    #region Coroutines
    // Coroutine setDistanceCoroutine;
    Coroutine dieCoroutine;
    Coroutine hitCoroutine;
    #endregion

    protected override void Awake() {
        base.Awake();
        #region Magic Effect Pool Generate
        magicEffectPool = new EffectPool("Magic", magicInstance, 10, 5, this.transform);
        #endregion Magic Effect Pool Generate

    }
    protected override void Start() {
        base.Start();
        StartCoroutine(Patrol());
    }
    protected override void InitializeState() {
        base.InitializeState();
        idleState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Idle", false);
        };
        chaseState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Chase", true);
        };
        chaseState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Chase", false);
        };
        attackState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Attack", true);
        };
        attackState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Attack", false);
        };
        magicState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Magic", true);
        };
        magicState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Magic", false);
        };
        hitState.OnActive += (prevState) => {
            monsterAnimator.SetTrigger("Hit");
        };
        dieState.OnActive += (prevState) => {
            monsterAnimator.SetBool("Die", true);
        };
        dieState.OnInactive += (nextState) => {
            monsterAnimator.SetBool("Die", false);
        };
    }
    protected override void Update() {
        base.Update();
        if(isDead) return;
        if(targetTransform != null) {
            AttackToTarget();
            SpellMagic();
        }
    }
    void AttackToTarget() {
        if(lastAttackTime > 0) lastAttackTime -= Time.deltaTime;
        else {
            if(remainingDistance <= attackDistance
            && !monsterStateMachine.Compare(ATTACK_STATE_TAG)
            && !monsterStateMachine.Compare(hitState)) {
                monsterStateMachine.ChangeState(attackState);
            }
        }
    }
    void SpellMagic() {
        if(lastMagicTime > 0) lastMagicTime -= Time.deltaTime;
        else {
            if((remainingDistance > attackDistance || Mathf.Abs(targetTransform.position.y - transform.position.y) > attackArea.bounds.y )
            && remainingDistance <= magicDistance
            && !monsterStateMachine.Compare(ATTACK_STATE_TAG)
            && !monsterStateMachine.Compare(hitState)) {
                nextMagicPoint = targetTransform.position;
                monsterStateMachine.ChangeState(magicState);
            }
        }
    }
    public void AnimationEvent_SpellEnd() {
        monsterStateMachine.ChangeState(idleState);
        lastMagicTime = magicInterval;
        Vector2 point = (Vector2)nextMagicPoint + new Vector2(0, .43f);
        GameObject effect = magicEffectPool.OutPool(point, null);
        effect.GetComponent<DeathBringer_Magic>().endEvent = () => {
            magicEffectPool.InPool(effect);
        };
    }
    void AnimationEvent_DamageTarget() {
        Collider2D collider = Physics2D.OverlapBox(damageArea.center, damageArea.bounds, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        lastAttackTime = attackInterval;
        if(collider && collider.tag == "Player") {
            Vector2 force = ((collider.transform.position - transform.position).normalized + Vector3.up*.5f) * 300f;
            collider.GetComponent<Player>().OnDamage(attackDamage, force);
        }
    }
    public void AnimationEvent_AttackEnd() {
        monsterStateMachine.ChangeState(idleState);
    }
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damageArea.center, damageArea.bounds);
    }

    public override void OnDamage(float damage, float duration=0) {
        if(isDead) return;
        base.OnDamage(damage, duration);
        if (hp <= 0) {
            Die();
        }
    }
    public override void OnDamage(float damage, Vector2 force, float duration=0) {
        if(isDead) return;
        monsterRigidbody.AddForce(force);
        if(hitCoroutine != null)
            StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitCoroutine(force, duration));
        OnDamage(damage, duration);
    }
    private IEnumerator HitCoroutine(Vector2 force, float duration) {
        if(duration > 0) {
            monsterStateMachine.ChangeState(hitState);
            yield return new WaitForSeconds(duration);
            monsterStateMachine.ChangeState(/* isDead ? dieState :  */idleState);
        }
    }

    protected override void Die() {
        base.Die();
        monsterSideUI.gameObject.SetActive(false);
        monsterStateMachine.ChangeState(dieState);
    }
    public void AnimationEvent_DieEnd() {
        gameObject.SetActive(false);
    }
    public IEnumerator Patrol() {
        while(!isDead) {
            DetectTarget();
            yield return new WaitForSeconds(.4f);
        }
    }
    protected override void DetectTarget() {
        Collider2D inner = Physics2D.OverlapCircle((Vector2)transform.position + detectRange.center, detectRange.radius, Player.DEFAULT_PLAYER_LAYERMASK);
        if(inner != null) {
            targetTransform = inner.transform;
            remainingDistance = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetTransform.position.x, 0));
        } else {
            MissTarget();
        }
    }
    protected override void MissTarget() {
        targetTransform = null;
    }
    protected override bool IsArrive() {
        return base.IsArrive();
    }
    protected override bool CanChase() {
        return !monsterStateMachine.Compare(ATTACK_STATE_TAG)
            && !monsterStateMachine.Compare(hitState)
            && !monsterStateMachine.Compare(dieState)
            && CheckDirection()
            && base.CanChase();
    }
    bool CheckDirection() {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + targetDirection.x*.7f, transform.position.y), Vector2.down, 1.1f);
        print(new Vector2(transform.position.x + targetDirection.x, transform.position.y - .1f));
        return !!hit;
    }
}