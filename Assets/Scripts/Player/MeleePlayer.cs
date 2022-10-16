using GameObjectState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePlayer : Player
{
    bool isAttackState;

    [SerializeField]
    BoxCollider2D meleeAttackRange;
    public override void BasicAttack()
    {
        base.BasicAttack();

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Weapon_MeleeAttack")) // 공격할 때 잠깐 공격 범위가 활성화
            meleeAttackRange.enabled = false;
        else
            meleeAttackRange.enabled = true;
    }

}
