using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using System.Drawing;
using Color = UnityEngine.Color;

public class Ronin : ChaseMonster {
    const string ATTACK_STATE_TAG = "tag:Attack";

    State attackState = new State("Attack", ATTACK_STATE_TAG);
    State hitState = new State("Hit");
    State jumpState = new State("Jump");
    State evasionState = new State("Evasion");

    private float lastAttackTime = 0f;
    private float attackInterval = 1.2f;
    private float attackDistance = 1.5f;

    
    private float evasionTimeInterval = 3f;
    private float lastEvasionTime = 0;

    [SerializeField] float attackDamage = 30f;
    [SerializeField] Range attackArea;

    float jumpMinimumHeight;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] GameObject virtualRotObject;

    [SerializeField] GiveExperience experience;

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
        jumpMinimumHeight = detectRange.radius / 2;
    }
    protected override void Start() {
        base.Start();
        StartCoroutine(Patrol());
        if (experience == null && TryGetComponent<GiveExperience>(out experience))
        {
            Debug.LogWarning($"There is any 'GiveExperience' component in {gameObject.name}");
        }
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
            monsterAnimator.SetTrigger("Jump");
        };
        evasionState.OnActive += (prevState) =>
        {
            monsterAnimator.SetTrigger("Dash");
        };

    }
    protected override void Update() {
        base.Update();
        if(isDead) return;
        if(targetTransform != null) {
            AttackToTarget();
        }
        EvasionTimer();
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
        int soundIndex = Random.Range(0, monsterAttackClip.Length);
        monsterSoundPlayer.PlayClip(monsterAttackClip[soundIndex]);
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

    public override void OnDamage(float damage, Vector2 force, Color color, float duration = .25f)
    {
        OnDamage(damage, force, duration);
    }

    public override void OnDamage(float damage, Vector2 force, float duration=0) {
        if (lastEvasionTime <= 0)
        {
            UIManager.instance.damageLog.LogDamage("MISS", transform.position, Color.white);
            lastEvasionTime = evasionTimeInterval;
            monsterStateMachine.ChangeState(evasionState);
            return;
        }
        else
        {
            if (isDead) return;
            UIManager.instance.damageLog.LogDamage((int)damage + "", transform.position, Color.white);
            monsterRigidbody.AddForce(force);
            if (hitCoroutine != null)
                StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(force, duration));
            OnDamage(damage, duration);
        }
    }
    private IEnumerator HitCoroutine(Vector2 force, float duration) {
            if (duration > 0)
            {
                monsterStateMachine.ChangeState(hitState);
                yield return new WaitForSeconds(duration);
                monsterStateMachine.ChangeState(/* isDead ? dieState :  */idleState);
            }
    }

    private void EvasionTimer()
    {
        if (lastEvasionTime > 0)
            lastEvasionTime -= Time.deltaTime;
    }

    public void AnimationEvent_EvasionStart()
    {
        monsterRigidbody.AddForce(new Vector2(-targetDirection.x * 15, 0), ForceMode2D.Impulse);
    }

    public void AnimationEvent_EvasionEnd()
    {
        monsterStateMachine.ChangeState(idleState);
    }

    protected override void Die() {
        base.Die();
        monsterSideUI.gameObject.SetActive(false);
        monsterStateMachine.ChangeState(dieState);
    }
    public void AnimationEvent_DieEnd() {
        Destroy(gameObject);
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
            Jump(CanJump(targetTransform));
        } else {
            MissTarget();
        }
    }
    protected override void MissTarget() {
        targetTransform = null;
    }
    private bool CanJump(Transform targetTransform) 
    {
        float dogCanJumpHeight = detectRange.radius - jumpMinimumHeight;
        float distanceY = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, targetTransform.position.y));
        float fovAngle = 45f;

        virtualRotObject.transform.eulerAngles = new Vector2(0, GetAngleFromVector(new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y)));
        return virtualRotObject.transform.eulerAngles.y > 90 - fovAngle && virtualRotObject.transform.eulerAngles.y < 90 + fovAngle && distanceY > dogCanJumpHeight; // leftAngle < targetAngle < rightAngle
                                                                                                                                                               // && 강아지가 점프할 수 있는 높이 = 반지름 - 1;
    }

    private void Jump(bool canJump)
    {
        float rayHeight = this.GetComponent<SpriteRenderer>().bounds.extents.y;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayHeight, LayerMask.GetMask("Groundable"));
        if (hit && canJump)
        {
            Rigidbody2D transformRigid = transform.GetComponent<Rigidbody2D>();
            jumpForce = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, targetTransform.position.y));
            if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Platform"))
            {
                monsterStateMachine.ChangeState(jumpState);
                transformRigid.AddForce(new Vector2(targetDirection.x * Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetTransform.position.x, 0)), Mathf.Sqrt(jumpForce * -2 * (Physics2D.gravity.y * transformRigid.gravityScale))), ForceMode2D.Impulse);
            }
                
        }
    }

    private int GetAngleFromVector(Vector2 dir)
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

    private void drawFov()
    {
        float lookingAngle = virtualRotObject.transform.eulerAngles.y;
        Vector3 rightDir = GetVectorFromAngle(virtualRotObject.transform.eulerAngles.y + 70 * 0.5f);
        Vector3 leftDir = GetVectorFromAngle(virtualRotObject.transform.eulerAngles.y - 70 * 0.5f);
        Vector3 lookDir = GetVectorFromAngle(lookingAngle);

        Debug.DrawRay(transform.position, rightDir * 5, Color.blue);
        Debug.DrawRay(transform.position, leftDir * 5, Color.blue);
        Debug.DrawRay(transform.position, lookDir * 5, Color.red);
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
    bool CheckDirection()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + targetDirection.x * .7f, transform.position.y), Vector2.down, 1.1f);
        return hit;
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