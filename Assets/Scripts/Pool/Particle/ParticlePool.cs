using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public class ParticlePool : Pool<GameObject> {

        public ParticlePool(string particleName, GameObject particleObject, int amount, int resizeAmount)
        : base(particleName, particleObject, amount, resizeAmount) {}
        
        public override GameObject OutPool(Vector2 point, Transform parent=null) {
            GameObject target;
            if(poolingQueue.Count <= 0) {
                return null;
            }
            target = poolingQueue.Dequeue();
            target.transform.position = point;
            target.transform.SetParent(parent);
            target.SetActive(true);
            return target;
        }
        public override void InPool(GameObject target, Transform originTransform) {
            target.SetActive(false);
            target.transform.SetParent(originTransform);
            target.transform.position = Vector2.zero;
            poolingQueue.Enqueue(target);
        }
    }
}