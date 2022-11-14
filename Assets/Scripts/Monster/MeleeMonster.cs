using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class MeleeMonster : Monster
{
    State idleState = new State("Idle");
    State moveState = new State("Move");
    State dieState = new State("Die");
    State attackState = new State("Attack");
    State hitState = new State("Hit");
    StateMachine monsterStateMachine;

    public Transform playerTransform;
    public float attackDamage = 15f;
    private float curTime;
    public float coolTime = 2f;
    public Transform attackAreaCenter;
    public Vector2 attackArea;
    float distance;

    SpriteRenderer rend;
    Rigidbody2D monsterRigidbody;
    Vector3 monsterPos;

    #region Coroutines
    Coroutine setDistanceCoroutine;
    Coroutine damageCoroutine;
    Coroutine hitCoroutine;
    #endregion

    [SerializeField] ParticleManager particleManager;

    protected override void Awake() {
        base.Awake();
        monsterRigidbody = GetComponent<Rigidbody2D>();
        if (TryGetComponent<StateMachine>(out monsterStateMachine)) {
            monsterStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Monster hasn't any 'StateMachine'.");
        }
        playerTransform = playerTransform==null ? GameObject.FindGameObjectWithTag("Player").transform : playerTransform;
        
        if(particleManager == null) {
            GameObject pgobj = GameObject.Find("Particle Manager");
            if(pgobj == null)
                Debug.LogWarning($"Particle Manager Script is null in {this.gameObject.name}");
            else
                particleManager = pgobj.GetComponent<ParticleManager>();
        }
    }
    [SerializeField] GameObject _damageEffect;
    protected override void Start() {

        base.Start();
        InitialState();
        rend = GetComponent<SpriteRenderer>();
        setDistanceCoroutine = StartCoroutine(SetDistance());
    }
    IEnumerator SetDistance() {
        distance = Vector2.Distance(transform.position, playerTransform.position);
        yield return new WaitForSeconds(.25f);
        setDistanceCoroutine = StartCoroutine(SetDistance());
    }

    void InitialState() {
        idleState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Idle", false);
        };
        moveState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Move", true);
        };
        moveState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Move", false);
        };
        attackState.OnActive += (nextState) => {
            monsterAnimator.SetBool("Attack", true);
        };
        attackState.OnInactive += (prevState) => {
            monsterAnimator.SetBool("Attack", false);
        };
        dieState.OnActive += (nextState) => {
            monsterAnimator.SetTrigger("Die");
        };
        hitState.OnActive += (prevState) => {
            monsterAnimator.SetTrigger("Hit");
        };
    }
    void MoveToTarget() {
        if(monsterStateMachine.Compare(attackState)
        || monsterStateMachine.Compare(hitState)) return;
        if(distance <= detectingDistance && distance > attackDistance) {
            monsterStateMachine.ChangeState(moveState);
            if (Mathf.Abs(playerTransform.position.x - transform.position.x) < 0.5f) {
                monsterStateMachine.ChangeState(idleState);
                return;
            }
            float dir = playerTransform.position.x - transform.position.x;
            dir = (dir < 0) ? -1 : 1;
            FaceTarget();
            transform.Translate(new Vector2(dir, 0) * moveSpeed * Time.deltaTime);
        } else {
            monsterStateMachine.ChangeState(idleState);
            FaceTarget();
        }
    }
    void DamageTarget() {
        Collider2D collider = Physics2D.OverlapBox(attackAreaCenter.position, attackArea, 0, Player.DEFAULT_PLAYER_LAYERMASK);
        if(collider && collider.tag == "Player") {
            Vector2 force = ((collider.transform.position - transform.position).normalized + Vector3.up*.5f) * 500f;
            collider.GetComponent<Player>().OnDamage(attackDamage, force);
        }
    }
    void AttackToTarget() {
        if(curTime > 0) curTime -=Time.deltaTime;
        if(distance <= attackDistance
        && !monsterStateMachine.Compare(hitState)) {
            if(curTime <= 0) {
                FaceTarget();
                monsterStateMachine.ChangeState(attackState);
                curTime = coolTime;
            }
        }
    }
    void ChangeIdle() {
        monsterStateMachine.ChangeState(idleState);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackAreaCenter.position, attackArea);
    }
    void FaceTarget() {
        if(!monsterStateMachine.Compare(attackState)
        && !monsterStateMachine.Compare(hitState)) {
            if(playerTransform.position.x - transform.position.x < 0) {
                LookAtX(-1);
            } else {
                LookAtX(1);
            }
        }
    }
    
    protected override void Update() {
        base.Update();
        MoveToTarget();
        AttackToTarget();
    }

    public override void OnDamage(float damage, float duration=0) {
        if(isDead) return;
        base.OnDamage(damage, duration);
        if (hp <= 0) {
            Die();
            return;
        }
    }
    public override void OnDamage(float damage, Vector2 force, float duration=0) {
        if(isDead) return;
        monsterRigidbody.AddForce(force);
        OnDamage(damage, duration);
        if(!isDead) {
            if(hitCoroutine != null)
                StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(force, duration));
        }
    }
    private IEnumerator HitCoroutine(Vector2 force, float duration) {
        if(duration > 0) {
            monsterStateMachine.ChangeState(hitState);
            yield return new WaitForSeconds(duration);
            monsterStateMachine.ChangeState(idleState);
        }
    }

    protected override void Die() {
        base.Die();
        monsterStateMachine.ChangeState(dieState);
        GetComponent<MeleeMonster>().enabled = false;
        Destroy(gameObject, 2f);
    }
}