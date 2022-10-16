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

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Weapon_MeleeAttack")) // ������ �� ��� ���� ������ Ȱ��ȭ
            meleeAttackRange.enabled = false;
        else
            meleeAttackRange.enabled = true;
    }

}
