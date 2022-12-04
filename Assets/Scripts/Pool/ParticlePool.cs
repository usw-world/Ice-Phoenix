using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public class ParticlePool : Pool<GameObject> {

        public ParticlePool(string particleName, GameObject particleObject, int amount, int resizeAmount, Transform originTransform=null)
        : base(particleName, particleObject, amount, resizeAmount, originTransform) {}
        
        public override GameObject OutPool(Vector2 point, Transform parent=null) {
            GameObject target;
            if(poolingQueue.Count <= 0) {
                base.RestorePool();
            }
            target = poolingQueue.Dequeue();
            target.SetActive(true);
            target.transform.position = point;
            target.transform.SetParent(parent);
            return target;
        }
        public override void InPool(GameObject target) {
            target.SetActive(false);
            target.transform.SetParent(originTransform);
            target.transform.position = Vector2.zero;
            poolingQueue.Enqueue(target);
        }
    }
}