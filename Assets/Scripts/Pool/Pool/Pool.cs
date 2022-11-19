using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public abstract class Pool<T> {
        public string particleName { get; protected set; }
        [SerializeField] public int amount { get; protected set; }
        [SerializeField] public int resizeAmount { get; protected set; }
        [SerializeField] protected T targetInstance;
        public T poolingObject { get; private set; }
        protected Queue<T> poolingQueue = new Queue<T>();
        public int Count {
            get {
                return poolingQueue.Count;
            }
        }
        public Pool(string particleName, T poolingObject, int amount=10, int resizeAmount=5) {
            this.particleName = particleName;
            this.amount = amount;
            this.resizeAmount = resizeAmount;
            this.poolingObject = poolingObject;
        }
        public abstract T OutPool(Vector2 point, Transform parent);
        public abstract void InPool(T target, Transform originTransform);
    }
}

