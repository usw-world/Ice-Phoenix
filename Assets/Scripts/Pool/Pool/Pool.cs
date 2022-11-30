using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public abstract class Pool<T> where T : Object {
        public string particleName { get; protected set; }
        [SerializeField] public int amount { get; protected set; }
        [SerializeField] public int resizeAmount { get; protected set; }
        [SerializeField] protected T targetInstance;
        public T poolingObject { get; private set; }
        protected Queue<T> poolingQueue = new Queue<T>();
        protected Transform originTransform;
        public int Count {
            get {
                return poolingQueue.Count;
            }
        }
        public Pool(string particleName, T poolingObject, int amount=10, int resizeAmount=5, Transform originTransform=null) {
            this.particleName = particleName;
            this.amount = amount;
            this.resizeAmount = resizeAmount;
            this.poolingObject = poolingObject;
            this.originTransform = originTransform;
            for(int i=0; i<amount; i++)
                InPool(Object.Instantiate<T>(poolingObject, originTransform));
        }
        public abstract T OutPool(Vector2 point, Transform parent);
        public abstract void InPool(T target);
        protected virtual void RestorePool() {
            for(int i=0; i<resizeAmount; i++)
                InPool(Object.Instantiate<T>(targetInstance, originTransform));
        }
    }
}

