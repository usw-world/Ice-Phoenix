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

    public Transform target;
    Monster monster;
    private void Awake()
    {
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
        print(distance);
        if(distance <= 5f) {
            monsterStateMachine.ChangeState(moveToplayerState);
        } else {
            monsterStateMachine.ChangeState(patrolState);
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

        };
    }
    void MoveToTarget()
    {
        float dir = target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        FaceTarget();
        transform.Translate(new Vector2(dir, 0) * monster.moveSpeed * Time.deltaTime);
    }

    void FaceTarget()
    {
        if (target.position.x - transform.position.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void patrol()
    {
        transform.Translate(new Vector2(-1, 0) * monster.moveSpeed * Time.deltaTime);
    }
}