using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class EffectPool : Pool<GameObject> {
    public EffectPool(string particleName, GameObject poolingObject, int amount = 10, int resizeAmount = 5, Transform originTransform=null)
    : base(particleName, poolingObject, amount, resizeAmount, originTransform) {}

    public override GameObject OutPool(Vector2 point, Transform parent=null) {
        GameObject effect;
        if(poolingQueue.Count <= 0) {
            base.RestorePool();
        }
        effect = poolingQueue.Dequeue();
        effect.transform.position = point;
        effect.transform.SetParent(parent);
        effect.SetActive(true);
        return effect;
    }
    public override void InPool(GameObject target) {
        target.SetActive(false);
        target.transform.SetParent(originTransform);
        target.transform.position = Vector2.zero;
        poolingQueue.Enqueue(target);
    }
}
