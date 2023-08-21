using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CmpLinkedList;

namespace AbilitySystem {
    public class AbilityManager : MonoBehaviour {
        static public AbilityManager instance;

        // [SerializeField] GameObject levelUpPrefab;

        [SerializeField] Player player;

        [SerializeField] private GameObject abilities;
        [SerializeField] private GameObject subAbilities;
        IntegratedList<Ability> currentAbilityChoices = new IntegratedList<Ability>();
        IntegratedList<Ability> subAbilityChoices = new IntegratedList<Ability>();
        public IntegratedList<Ability> playersAbilities { get; private set; } = new IntegratedList<Ability>();

        // [SerializeField] Ability testAbility;

        private void Awake() {
            if(instance != null) Destroy(this.gameObject);
            else instance = this;

            if(player == null) {
                GameObject pobj = GameObject.FindGameObjectWithTag("Player");
                if(pobj != null)
                    player = pobj.GetComponent<Player>();
            }
            
            foreach(Ability ability in abilities.GetComponentsInChildren<Ability>()) {
                if(!currentAbilityChoices.Contains(ability))
                    currentAbilityChoices.Push(ability);
            }
            foreach(Ability ability in subAbilities.GetComponentsInChildren<Ability>()) {
                if(!subAbilityChoices.Contains(ability))
                    subAbilityChoices.Push(ability);
            }
        }
        void Start() {
            RefreshAbilityUIs();
        }
        public void Update() {
            /* ------------------- */
            if(Input.GetKeyDown(KeyCode.Y)) {
                player.IncreaseExp(500);
            }
            /* ------------------- */
        }
        public void OfferChoices() {
            int index;

            IntegratedList<Ability> list;
            if(playersAbilities.Count < 10) list = currentAbilityChoices.Copy();
            else                            list = new IntegratedList<Ability>(); 
            
            // print(list.Count);

            int lakcCount = AbilityChoicesUI.CHOISE_NUMBER - list.Count;
            while(lakcCount>0) {
                index = Random.Range(0, subAbilityChoices.Count);
                list.Push(subAbilityChoices[index]);
                lakcCount --;
            }
            for(int i=0; i<AbilityChoicesUI.CHOISE_NUMBER; i++) {
                index = Random.Range(0, list.Count);
                UIManager.instance.abilityChoicesUI.SetChoice(i, list.Shift(index));
            }
            UIManager.instance.abilityChoicesUI.ShowChoices();
        }
        public void AddAbility(Ability ability) {
            Ability target;
            if(currentAbilityChoices.Find(ability, out target)
            || subAbilityChoices.Find(ability, out target)) {
                if(!playersAbilities.Contains(target)) {
                    // Get the new ability case **
                    playersAbilities.Push(target);
                    foreach(Ability da in target.derivedAbility) {
                        RestoreAbility(da);
                    }
                    target.OnGetAbility();
                } else {
                    // Level Up case **
                    target.LevelUp();
                }
                if(target.isMaxLevel) {
                    currentAbilityChoices.Remove(target);
                }
            }
            RefreshAbilityUIs();
        }
        public void RemoveAbility(Ability ability) {
            ability.OnReleaseAbility();
            playersAbilities.Remove(ability);
            RefreshAbilityUIs();
        }
        public void RestoreAbility(Ability ability) {
            currentAbilityChoices.Push(ability);
        }
        public void AbilityLevelUp(Ability ability) {
            ability.LevelUp();
        }
        public void RefreshAbilityUIs() {
            UIManager.instance.screenUI.UpdateAbilityIcons();
            UIManager.instance.playerStatusUI.UpdateAbilityUI();
        }
    }
}
