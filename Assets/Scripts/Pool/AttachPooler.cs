using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class AttachPooler : Pool<Attach> {
    public AttachPooler(string particleName, Attach poolingAttach, int amount = 10, int resizeAmount = 5, Transform originTransform=null)
    : base(particleName, poolingAttach, amount, resizeAmount, originTransform) {}

    public override Attach OutPool(Vector2 point, Transform parent=null) {
        Attach effect;
        if(poolingQueue.Count <= 0) {
            base.RestorePool();
        }
        effect = poolingQueue.Dequeue();
        effect.transform.position = point;
        effect.transform.SetParent(parent);
        effect.gameObject.SetActive(true);
        return effect;
    }
    public override void InPool(Attach target) {
        target.gameObject.SetActive(false);
        target.transform.SetParent(originTransform);
        target.transform.position = Vector2.zero;
        poolingQueue.Enqueue(target);
    }
}
