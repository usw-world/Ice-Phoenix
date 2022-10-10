using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;
using System;

public class Monster : MonoBehaviour {    
    public Animator MonsterAnim;
    public GameObject Player;
    public GameObject monster;
    public string enemyName;
    public float moveSpeed;    
    public float fieldOfVision;
    public int attackRange;
    public int maxHp;
    public int nowHp;
    private void SetEnemyStatus(string _enemyName,float _moveSpeed,float _fieldOfVision, int _attackRange,int _maxHp,int _nowHp)
    {
        enemyName = _enemyName;        
        moveSpeed = _moveSpeed;
        fieldOfVision = _fieldOfVision;
        attackRange = _attackRange;
        maxHp = _maxHp;
        nowHp = _nowHp;
    }        

    void Die()
    {        
        
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    // private void OnCollisionEnter2D(Collision2D col)
    // {
    //     if (col.gameObject.CompareTag("Player"))
    //     {
    //         col.gameObject.GetComponent<Player>().OnDamage(1);
    //         Debug.Log("체력감소");
    //     }
    // }   
}
