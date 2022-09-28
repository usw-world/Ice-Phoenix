using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAi : MonoBehaviour
{
    public Transform target;
    Monster monster;
    void Start()
    {
        monster = GetComponent<Monster>();        
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position); //몬스터 플레이어 거리
        print(distance);
        if(distance <= 5f)
        {
            MoveToTarget();
        }
        if (distance <= monster.fieldOfVision) //몬스터 시야 범위
        {
            FaceTarget();            
        }        
    }

    void MoveToTarget()
    {
        float dir = target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        transform.Translate(new Vector2(dir, 0) * monster.moveSpeed * Time.deltaTime);
    }

    void FaceTarget()
    {
        if (target.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}