using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem {
    public abstract class Ability : MonoBehaviour {
        [SerializeField] Image abilityImage;
        List<Ability> preAbility;
        bool isUnlock = false;

        [SerializeField] Dictionary<string, GameObject> map;
        
        protected abstract int maxLevel { get; }
        protected int a = 0;
        [SerializeField] protected int currentLevel = 0;
        protected virtual void Start() {}

        protected class ValueOnLevel<T> {
            private List<T> values = new List<T>();
            public T this[int key] {
                get {
                    return values[key];
                }
            }
        }
    }
}
