using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class MonsterPooler : Pool<GameObject> {
    public MonsterPooler(string particleName, GameObject poolingObject, int amount = 10, int resizeAmount = 5, Transform originTransform=null)
    : base(particleName, poolingObject, amount, resizeAmount, originTransform) {}

    public override GameObject OutPool(Vector2 point, Transform parent=null) {
        GameObject monster;
        if(poolingQueue.Count <= 0) {
            base.RestorePool();
        }
        monster = poolingQueue.Dequeue();
        monster.transform.position = point;
        monster.transform.SetParent(parent);
        monster.SetActive(true);
        return monster;
    }
    public override void InPool(GameObject target) {
        target.SetActive(false);
        target.transform.SetParent(originTransform);
        target.transform.position = Vector2.zero;
        poolingQueue.Enqueue(target);
    }
}
