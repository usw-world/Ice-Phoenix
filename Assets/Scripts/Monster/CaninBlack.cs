using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor;

public class CaninBlack : ChaseMonster {
    const string ATTACK_STATE_TAG = "tag:Attack";

    State attackState = new State("Attack", ATTACK_STATE_TAG);
    State hitState = new State("Hit");
    State jumpState = new State("Jump");

    private float lastAttackTime = 0f;
    private float attackInterval = 1.2f;
    private float attackDistance = 1.5f;

    [SerializeField] float attackDamage = 30f;
    [SerializeField] Range attackArea;

    [SerializeField] float jumpLimit = 10f;
    [SerializeField] GameObject virtualRotObject;

    Range damageArea {
        get {
            return new Range(
                (Vector2)transform.position + attackArea.center * new Vector2(transform.localScale.x, 1),
                attackArea.bounds
            );
        }
    }

    protected Vector2 targetDirection
    {
        get
        {
            return new Vector2(
                targetTransform != null && targetTransform.position.x - transform.position.x > 0 ? -1 : 1,
                targetTransform != null && targetTransform.position.y - transform.position.y > 0 ? -1 : 1
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

    }
    protected override void Start() {
        base.Start();
        StartCoroutine(Patrol());
    }
    protected override void InitializeState() {
        chaseState.OnStay += () => {
            LookAtX(targetDirection.x);
            if (CanChase())
            {
                Vector2 moveSpace = new Vector2(targetDirection.x, 0) * moveSpeed * Time.deltaTime;
                transform.Translate(-moveSpace);
                remainingDistance -= moveSpace.magnitude;
            }
            else
            {
                monsterStateMachine.ChangeState(idleState);
            }
        };

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
        hitState.OnActive += (prevState) => {
            monsterAnimator.SetTrigger("Hit");
        };
        dieState.OnActive += (prevState) => {
            monsterAnimator.SetBool("Die", true);
        };
        dieState.OnInactive += (nextState) => {
            monsterAnimator.SetBool("Die", false);
        };
        jumpState.OnActive += (prevState) =>
        {
            monsterAnimator.SetBool("Jump", true);
        };
        jumpState.OnInactive += (nextState) =>
        {
            monsterAnimator.SetBool("Jump", false);
        };
    }
    protected override void Update() {
        base.Update();
        if(isDead) return;
        if(targetTransform != null) {
            AttackToTarget();
        }
    }
    void AttackToTarget() {
        if (lastAttackTime > 0)
        {
            lastAttackTime -= Time.deltaTime;
        }
        else
        {
            if (remainingDistance <= attackDistance
            && !monsterStateMachine.Compare(ATTACK_STATE_TAG)
            && !monsterStateMachine.Compare(hitState))
            {
                monsterStateMachine.ChangeState(attackState);
            }
        }
    }

    void AnimationEvent_DamageTarget() {
        Collider2D collider = Physics2D.OverlapBox(damageArea.center, damageArea.bounds, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        lastAttackTime = attackInterval;
        if((collider && collider.tag == "Player") 
         && !CheckWallBetween(collider.transform, attackArea.center.y)) {
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
            Jump(CanJump(targetTransform, detectRange.radius));
        } else {
            MissTarget();
        }
    }
    protected override void MissTarget() {
        targetTransform = null;
    }
    private bool CanJump(Transform targetTransform, float detectRadius) 
    {
        float viewAngle = 45f;
        virtualRotObject.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(new Vector3(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y, 0)));
        
        Vector3 lookDir = GetVectorFromAngle(virtualRotObject.transform.eulerAngles.z);
        Vector3 rightDir = GetVectorFromAngle(virtualRotObject.transform.eulerAngles.z + viewAngle * .5f);
        Vector3 leftDir = GetVectorFromAngle(virtualRotObject.transform.eulerAngles.z - viewAngle * .5f);

        Debug.DrawRay(transform.position, lookDir * detectRadius, Color.blue, detectRadius);
        Debug.DrawRay(transform.position, rightDir * detectRadius, Color.red, detectRadius);
        Debug.DrawRay(transform.position, leftDir * detectRadius, Color.red, detectRadius);

        return virtualRotObject.transform.eulerAngles.z > 60 && virtualRotObject.transform.eulerAngles.z < 120;
    }

    private void Jump(bool canJump)
    {
        if (canJump)
        {
            monsterStateMachine.ChangeState(jumpState);
        }
            
    }

    public void AnimationEvent_JumpEnd()
    {
        monsterStateMachine.ChangeState(idleState);
    }

    private int GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        int angle = Mathf.RoundToInt(n);

        return angle;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
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
        //print(new Vector2(transform.position.x + targetDirection.x, transform.position.y - .1f));
        return !!hit;
    }

    private bool CheckWallBetween(Transform target, float pointY)
    {
        Collider2D collider;
        if (target.TryGetComponent<Collider2D>(out collider))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)transform.position + new Vector2(0, pointY),
                new Vector2(transform.localScale.x, 0),
                Vector2.Distance((Vector2)transform.position, target.position) - target.GetComponent<Collider2D>().bounds.size.x / 2,
                1 << 6
            );
            print((Vector2)transform.position + new Vector2(0, pointY) + " ~ " + ((Vector2)transform.position + new Vector2(0, pointY) + (Vector2.Distance((Vector2)transform.position, target.position) - target.GetComponent<Collider2D>().bounds.size.x / 2) * new Vector2(transform.localScale.x, 0)));
            return !!hit && hit.collider.tag == "Ground";
        }
        return false;
    }
}