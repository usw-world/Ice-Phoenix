using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class MonsterAi : MonoBehaviour
{
    State idleState = new State("Idle");
    State patrolState = new State("Patrol");
    State moveState = new State("Move");
    State dieState = new State("Die");
    State attackState = new State("Attack");
    StateMachine monsterStateMachine;

    public Transform target;
    public int nowMove;
    public int nextMove;    
    private float curTime;
    public float coolTime = 2f;
    public Transform pos;
    public Vector2 boxSize;
    float distance;

    SpriteRenderer rend;
    public Animator monsterAnim;
    Rigidbody2D rigid;
    Monster monster;
    Vector3 monsterPos;

    Coroutine setDistanceCoroutine;
    Coroutine damageCoroutine;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        // Invoke("Think", 5);
        if (TryGetComponent<StateMachine>(out monsterStateMachine)) {
            monsterStateMachine.SetIntialState(new State("Idle"));
        } else {
            Debug.LogError("Monster hasn't any 'StateMachine'.");
        }
    }
    void Start()
    {
        InitialState();
        // MonsterAnim = monster.MonsterAnim;
        monster = GetComponent<Monster>();
        rend = GetComponent<SpriteRenderer>();
        setDistanceCoroutine = StartCoroutine(SetDistance());
    }
    IEnumerator SetDistance()
    {
        distance = Vector2.Distance(transform.position, target.position);
        yield return new WaitForSeconds(.5f);
        setDistanceCoroutine = StartCoroutine(SetDistance());
    }
    IEnumerator Damage()
    {
        yield return new WaitForSeconds(.2f);
    }

    void InitialState()
    {
        idleState.OnActive += () => {
            monsterAnim.SetBool("idle", true);
        };
        idleState.OnInactive += () => {
            monsterAnim.SetBool("idle", false);
        };
        moveState.OnActive += () => {
            monsterAnim.SetBool("moving", true);
        };
        moveState.OnInactive += () => {
            monsterAnim.SetBool("moving", false);
        };
        attackState.OnActive += () => {
            monsterAnim.SetTrigger("attack");
        };
        // patrolState.OnStay += () => {
        //     patrol();
        // };
        dieState.OnActive += () => {
            monsterAnim.SetTrigger("die");
        };
    }
    void MoveToTarget()
    {
        if(monsterStateMachine.Compare(attackState)) return;
        if(distance <= monster.fieldOfVision) {
            monsterStateMachine.ChangeState(moveState);
            if (Mathf.Abs(target.position.x - transform.position.x) < 0.5f) {
                monsterStateMachine.ChangeState(idleState);
                return;
            }
            float dir = target.position.x - transform.position.x;
            dir = (dir < 0) ? -1 : 1;
            FaceTarget();
            transform.Translate(new Vector2(dir, 0) * monster.moveSpeed * Time.deltaTime);
        } else {
            monsterStateMachine.ChangeState(idleState);
        }

        // Vector2 frontVec = new Vector2(transform.position.x + dir, transform.position.y);
        // Debug.DrawRay(frontVec, new Vector3(0,-1.005f,0), new Color(0,1,0));
        // RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector2.down,1.005f,64);

        // if (raycast.collider == null) {
        //     Turn();
        //     Debug.Log("There is no Ground in front of Monster");
        //     return;
        // }
    }
    void DamageTarget()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<Player>().OnDamage();
            }
        } 
    }
    void AttackToTarget()
    {
        if(curTime>0) curTime -=Time.deltaTime;
        if (distance <= monster.attackRange && curTime <= 0)
        {
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
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    void FaceTarget()
    {
        if(!monsterStateMachine.Compare(attackState))
        {
            if (target.position.x - transform.position.x < 0.1f) {
                transform.localScale = new Vector3(-1.8f, 1.8f, 1);
            }
            else
            {
                transform.localScale = new Vector3(1.8f, 1.8f, 1);
            }
            // if (Mathf.Abs(target.position.x - transform.position.x) < 0.5f)
            // {
            //     MonsterAnim.SetBool("moving", false);
            //     // return;
            // }
        }                
    }




    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            monster.nowHp -= 1;
            Debug.Log(monster.nowHp);
        }
    }










    public int damage = 4;
    public float cooltime2;
    private float currenttime;
    
    void Update()
    {        
        MoveToTarget();
        AttackToTarget();
        Die();

        Collider2D[] collider = Physics2D.OverlapBoxAll(pos.position, new Vector2(1f, 1f), 1);

        if(collider != null)
        {
            for(int i = 0; i < collider.Length; i++)
            {
                if(currenttime <= 0)
                {
                    if(collider[i].tag == "Player")
                    {
                        currenttime = cooltime2;
                    }
                }
            }
            currenttime -= Time.deltaTime;
        }
    }

    
    





    
    







    //public void FixedUpdate() {
    // Vector2 frontVec = new Vector2(transform.position.x + nextMove, transform.position.y);
    // Debug.DrawRay(frontVec, new Vector3(0,-1,0), new Color(0,1,0));
    // RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector3.down,1,LayerMask.GetMask("Ground"));
    // if(raycast.collider == null)
    // {
    //     // Debug.Log("There is no Ground in front of Monster");
    //     nextMove = nextMove*(-1);
    //     CancelInvoke();
    //     Invoke("Think",5);
    // }
    //}
    // void patrol()
    // {
    //     float distance = Vector3.Distance(transform.position, target.position);
    //     if(distance <= 5f) {
    //         monsterStateMachine.ChangeState(moveToplayerState);
    //     }

    //     Vector2 frontVec = new Vector2(transform.position.x + nextMove, transform.position.y);
    //     Debug.DrawRay(frontVec, new Vector3(0,-1.005f,0), new Color(0,1,0));
    //     RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector2.down,1.005f,64);        

    //     if (raycast.collider == null) {
    //         Debug.Log("There is no Ground in front of Monster");                        
    //         Turn();
    //     }

    //     transform.Translate(new Vector2(nextMove, 0) * monster.moveSpeed * Time.deltaTime);
    //     if(nextMove < 0) {            
    //         transform.localScale = new Vector3(-1, 1, 1);
    //         Debug.DrawRay(transform.position, new Vector3(-1,0,0), new Color(0,1,0));
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(-1,0,0),1,64);
    //         if(hit.collider != null) {
    //             Turn();
    //         }
    //     } else {
    //         transform.localScale = new Vector3(1, 1, 1);
    //         Debug.DrawRay(transform.position, new Vector3(1,0,0), new Color(0,1,0));
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(1,0,0),1,64);
    //         if(hit.collider != null) {
    //             Turn();
    //         }
    //     }
    // }
    // void Turn()
    // {
    //     nextMove = nextMove*(-1);
    //     CancelInvoke();
    //     Invoke("Think",5);
    // }
    // void Think()
    // {
    //     nextMove = Random.Range(-1,2);        
    //     float thinkTime = Random.Range(2f,5f);
    //     Invoke("Think", thinkTime);
    // }

    void Die()
    {
        if (monster.nowHp <= 0)
        {
            monsterStateMachine.ChangeState(dieState);            
            GetComponent<MonsterAi>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(monster.monster,2f);
        }
    }
}