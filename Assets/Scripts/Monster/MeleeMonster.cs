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

    Coroutine setDistanceCoroutine;
    Coroutine damageCoroutine;

    protected override void Awake() {
        base.Awake();
        monsterRigidbody = GetComponent<Rigidbody2D>();
        if (TryGetComponent<StateMachine>(out monsterStateMachine)) {
            monsterStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Monster hasn't any 'StateMachine'.");
        }
        playerTransform = playerTransform==null ? GameObject.FindGameObjectWithTag("Player").transform : playerTransform;
    }
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
        idleState.OnActive += () => {
            monsterAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive += () => {
            monsterAnimator.SetBool("Idle", false);
        };
        moveState.OnActive += () => {
            monsterAnimator.SetBool("Move", true);
        };
        moveState.OnInactive += () => {
            monsterAnimator.SetBool("Move", false);
        };
        attackState.OnActive += () => {
            monsterAnimator.SetTrigger("Attack");
        };
        dieState.OnActive += () => {
            monsterAnimator.SetTrigger("Die");
        };
        hitState.OnActive += () => {
            monsterAnimator.SetTrigger("Hit");
        };
    }
    void MoveToTarget() {
        if(monsterStateMachine.Compare(attackState)) return;
        if(distance <= detectingDistance) {
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
        if(distance <= attackDistance && curTime <= 0) {
            monsterStateMachine.ChangeState(attackState);
            FaceTarget();
            curTime = coolTime;
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
        if(!monsterStateMachine.Compare(attackState)) {
            if(playerTransform.position.x - transform.position.x < 0.1f) {
                transform.localScale = new Vector3(-1.8f, 1.8f, 1);
            } else {
                transform.localScale = new Vector3(1.8f, 1.8f, 1);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "meleeRange") {
            hp -= 10;
        }
    }   
    
    void Update() {
        MoveToTarget();
        AttackToTarget();
        Die();
    }

    protected override void Die() {
        if (hp <= 0)
        {
            monsterStateMachine.ChangeState(dieState);
            GetComponent<MeleeMonster>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(gameObject, 2f);
        }
    }
}