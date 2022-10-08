using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using System;

public class Monster : MonoBehaviour {    
    public GameObject Player;
    public GameObject monster;
    public string enemyName;
    public float moveSpeed;    
    public float fieldOfVision;
    public int maxHp;
    public int nowHp;
    private void SetEnemyStatus(string _enemyName,float _moveSpeed,float _fieldOfVision,int _maxHp,int _nowHp)
    {
        enemyName = _enemyName;        
        moveSpeed = _moveSpeed;
        fieldOfVision = _fieldOfVision;
        maxHp = _maxHp;
        nowHp = _nowHp;
    }
    

    void Die()
    {        
        GetComponent<MonsterAi>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(monster,2f);
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Player>().OnDamage(1);
            Debug.Log("체력감소");
        }
    }

    
}
