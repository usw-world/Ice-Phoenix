using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public class ParticlePooler2D : Pooler<GameObject> {

        private void Start() {}
        private void Update() {
            if(Input.GetMouseButtonDown(0)) {
                GameObject t = Call(Vector2.zero, 2);
            }
        }
        protected override void InitializePool() {
            poolingQueue = new Queue<GameObject>();
            
            for(int i=0; i<amount; i++) {
                GameObject particle = Instantiate(targetInstance, parentObject);
                particle.SetActive(false);
                poolingQueue.Enqueue(particle);
            }
            poolingObjects = poolingQueue.ToArray();
        }
        protected override GameObject Call(Vector2 point, Transform parent=null) {
            GameObject target = poolingQueue.Dequeue();
            target.transform.SetParent(parent);
            target.SetActive(true);
            return target;
        }
        protected override GameObject Call(Vector2 point, int second, Transform parent=null) {
            GameObject target = Call(point, parent);
            Release(target, second);
            return target;
        }
        protected override void Release(GameObject target) {
            target.transform.position = Vector2.zero;
            target.transform.SetParent(parentObject);
            target.SetActive(false);
            poolingQueue.Enqueue(target);
        }
        protected override void Release(GameObject target, int second) {
            StartCoroutine(
                Utility.CoroutineTask(() => {
                    Release(target);
                }, second)
            );
        }
    }
}