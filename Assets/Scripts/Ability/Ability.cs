using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CmpLinkedList;
using System;

namespace AbilitySystem {
    public abstract class Ability : MonoBehaviour {
        public bool playerHas { get; protected set; } = false;
        public IntegratedList<Ability> derivedAbility = new IntegratedList<Ability>();
        [SerializeField] Ability[] derivedAbilityList;
        [SerializeField] protected Sprite abilityImage;
        [SerializeField] protected string abilityName;
        [TextArea(3, 10)][SerializeField] protected string description/*  = "잃은 체력이 비례해 추가 피해를 가합니다.\n추가 피해량 : 15% / 40% / 65%" */;
        [TextArea(3, 10)][SerializeField] protected string annotation/*  = "(현재 체력 20%에서 최고 상승량)" */;
        [TextArea(3, 10)][SerializeField] protected string abilitySummary;

        public Ability GetAbilityInstance { get { return this; } }
        public virtual Sprite GetAbilityImage { get { return abilityImage; } }
        public virtual string GetAbilityName { get { return abilityName; } }
        public virtual int GetLevel { get { return level; } }
        public virtual string GetDescription { get { return description; } }
        public virtual string GetAnnotation { get { return annotation; } }
        public virtual string GetSummary { get { return abilitySummary; } }
        bool isUnlock = false;
        
        public abstract int maxLevel { get; }
        [SerializeField] private int currentLevel = 0;
        private void Start() {
            level = 0;
        }
        protected virtual int level {
            get {
                return currentLevel<0 
                    ? 0 
                    : currentLevel>=maxLevel 
                        ? maxLevel-1
                        : currentLevel;
            }
            set {
                currentLevel = value<0 
                    ? 0 
                    : value>=maxLevel 
                        ? maxLevel-1
                        : value;
            }
        }

        public virtual void OnGetAbility() {
            playerHas = true;
        }
        public abstract void OnReleaseAbility();
        public void LevelUp() {
            level++;
        }
        public bool Equals(Ability other) {
            return other != null && abilityName == other.abilityName;
        }
        public override bool Equals(object other) {
            return Equals(other as Ability);
        }
        public override int GetHashCode() {
            HashCode hash = new HashCode();
            hash.Add(abilityName);
            return hash.ToHashCode();
        }
    }
}
