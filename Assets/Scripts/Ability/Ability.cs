using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem {
    public abstract class Ability : MonoBehaviour {
        [SerializeField] Image abilityImage;
        List<Ability> preAbility;
        bool isUnlock = false;
        
        protected abstract int maxLevel { get; }
        protected int currentLevel = 1;
        public int level {
            get {
                return currentLevel<0 
                    ? 0 
                    : currentLevel>maxLevel 
                        ? maxLevel 
                        : currentLevel;
            }
            protected set {
                currentLevel = value<0 
                    ? 0 
                    : value>maxLevel 
                        ? maxLevel 
                        : value;
            }
        }
        public abstract void OnGetAbility();
        public abstract void OnReleaseAbility();
    }
}
