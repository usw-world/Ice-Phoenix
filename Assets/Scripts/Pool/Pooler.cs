using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool {
    public abstract class Pooler<T> : MonoBehaviour {
        [SerializeField] protected int amount = 10;
        [SerializeField] protected int reszieAmount = 10;
        [SerializeField] protected Transform parentObject = null;
        [SerializeField] protected T targetInstance;
        protected Queue<T> poolingQueue;
        protected virtual void Awake() {
            InitializePool();
        }
        protected abstract void InitializePool();
        protected abstract void Generate(int count);
        protected abstract T Call(Vector2 point, Transform parent);
        protected abstract T Call(Vector2 point, int time_ms, Transform parent);
        protected abstract void Release(T target);
        protected abstract void Release(T target, int time_ms);
    }
}

