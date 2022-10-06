using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
public class MonsterAi : MonoBehaviour
{
    State patrolState = new State("Patrol");
    State moveToplayerState = new State("MoveToPlayer");
    State dieState = new State("Die");
    StateMachine monsterStateMachine;

    Rigidbody2D rigid;
    public Transform target;
    public int nowMove;
    public int nextMove;
    Monster monster;
    Vector3 monsterPos;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", 5);
        if (TryGetComponent<StateMachine>(out monsterStateMachine)) {
            monsterStateMachine.SetIntialState(new State("Patrol"));
        } else {
            Debug.LogError("Monster hasn't any 'StateMachine'.");
        }
    }
    void Start()
    {
        InitialState();
        monster = GetComponent<Monster>();        
    }
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position); //몬스터 플레이어 거리
        // print(distance);
        if(distance <= 5f) {
            monsterStateMachine.ChangeState(moveToplayerState);
        } else {
            monsterStateMachine.ChangeState(patrolState);
        }
        if(monster.nowHp <= 0) {
            monsterStateMachine.ChangeState(dieState);
        }
    }
    
    void InitialState()
    {
        moveToplayerState.OnStay += () => {
            MoveToTarget();            
        };
        patrolState.OnStay += () => {
            patrol();
        };
        dieState.OnActive += () => {
            Die();
        };
    }
    void MoveToTarget()
    {                
        float dir = target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        FaceTarget();
        transform.Translate(new Vector2(dir, 0) * monster.moveSpeed * Time.deltaTime);
        
        Vector2 frontVec = new Vector2(transform.position.x + dir, transform.position.y);
        Debug.DrawRay(frontVec, new Vector3(0,-1.005f,0), new Color(0,1,0));
        RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector2.down,1.005f,64);

        if (raycast.collider == null) {
            Turn();
            Debug.Log("There is no Ground in front of Monster");
            return;
        }
    }

    void FaceTarget()
    {
        if (target.position.x - transform.position.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void FixedUpdate() {
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
    }
    void patrol()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= 5f) {
            monsterStateMachine.ChangeState(moveToplayerState);
        }

        Vector2 frontVec = new Vector2(transform.position.x + nextMove, transform.position.y);
        Debug.DrawRay(frontVec, new Vector3(0,-1.005f,0), new Color(0,1,0));
        RaycastHit2D raycast = Physics2D.Raycast(frontVec, Vector2.down,1.005f,64);        
        
        if (raycast.collider == null) {
            Debug.Log("There is no Ground in front of Monster");                        
            Turn();
        }

        transform.Translate(new Vector2(nextMove, 0) * monster.moveSpeed * Time.deltaTime);
        if(nextMove < 0) {            
            transform.localScale = new Vector3(-1, 1, 1);
            Debug.DrawRay(transform.position, new Vector3(-1,0,0), new Color(0,1,0));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(-1,0,0),1,64);
            if(hit.collider != null) {
                Turn();
            }
        } else {
            transform.localScale = new Vector3(1, 1, 1);
            Debug.DrawRay(transform.position, new Vector3(1,0,0), new Color(0,1,0));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(1,0,0),1,64);
            if(hit.collider != null) {
                Turn();
            }
        }
    }
    void Turn()
    {
        nextMove = nextMove*(-1);
        CancelInvoke();
        Invoke("Think",5);
    }
    void Idle()
    {
        
    }
    void Think()
    {
        nextMove = Random.Range(-1,2);        
        float thinkTime = Random.Range(2f,5f);
        Invoke("Think", thinkTime);
    }

    void Die()
    {        
        GetComponent<MonsterAi>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(monster.monster,2f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            monster.nowHp -= 20;
            print("충돌");
        }
    }
}