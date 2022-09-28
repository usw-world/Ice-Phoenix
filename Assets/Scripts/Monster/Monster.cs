using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using System;

public class Monster : MonoBehaviour {    
    public GameObject monster;
    public string enemyName;
    public float moveSpeed;    
    public float fieldOfVision;
    private void SetEnemyStatus(string _enemyName,float _moveSpeed,float _fieldOfVision)
    {
        enemyName = _enemyName;        
        moveSpeed = _moveSpeed;
        fieldOfVision = _fieldOfVision;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            print("충돌");
            Destroy(monster,2f);
            Die();
        }
    }

    void Die()
    {        
        GetComponent<MonsterAi>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(GetComponent<Rigidbody2D>());
    }

    void Start()
    {
        
    }

    void Update()
    {

    }
    
}
