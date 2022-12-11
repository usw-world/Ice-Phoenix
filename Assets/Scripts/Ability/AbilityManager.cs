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

        [SerializeField] List<Ability> allAbilities = new List<Ability>();
        [SerializeField] List<Ability> abilitiesForEmpty = new List<Ability>();
        IntegratedList<Ability> currentAbilityChoices = new IntegratedList<Ability>();
        public IntegratedList<Ability> playersAbilities { get; private set; } = new IntegratedList<Ability>();

        [SerializeField] Ability testAbility;

        private void Awake() {
            if(instance != null) Destroy(this.gameObject);
            else instance = this;

            if(player == null) {
                GameObject pobj = GameObject.FindGameObjectWithTag("Player");
                if(pobj != null)
                    player = pobj.GetComponent<Player>();
            }
            /*  */
            foreach(Ability ability in allAbilities) {
                if(!currentAbilityChoices.Contains(ability))
                    currentAbilityChoices.Push(ability);
            }
            /*  */
        }
        void Start() {
            /*  */
            // AddAbility(testAbility);
            /*  */
            RefreshAbilityUIs();
        }
        public void Update() {
            /* ------------------- */
            if(Input.GetKeyDown(KeyCode.Y)) {
                string str = "";
                foreach(Ability a in playersAbilities) {
                    str += a.GetAbilityName+" ";
                    str += a.GetLevel+", ";
                }
                print(str);
            }
            /* ------------------- */
        }
        public void OfferChoices() {
            int index;
            int subChoicesCount = 0;
            Queue<Ability> choices = new Queue<Ability>();
            for(int i=0; i<3; i++) {
                if(currentAbilityChoices.Count>0 && playersAbilities.Count<10) {
                    index = Random.Range(0, currentAbilityChoices.Count);
                    choices.Enqueue(currentAbilityChoices.Shift(index));
                } else {
                    subChoicesCount ++;
                    index = Random.Range(0, abilitiesForEmpty.Count);
                    choices.Enqueue(abilitiesForEmpty[index]);
                }
            }
            for(int i=0; i<3; i++) {
                Ability choice = choices.Dequeue();
                if(subChoicesCount >= 3-i) {
                    subChoicesCount --;
                } else {
                    currentAbilityChoices.Push(choice);
                }
                UIManager.instance.abilityChoicesUI.SetChoice(i, choice);
            }
            UIManager.instance.abilityChoicesUI.ShowChoices();
        }
        public void AddAbility(Ability ability) {
            Ability target;
            if(currentAbilityChoices.Shift(ability, out target)) {
                if(!playersAbilities.Contains(target)) { // Get the new ability case
                    playersAbilities.Push(target);
                    foreach(Ability da in target.derivedAbility) {
                        RestoreAbility(da);
                    }
                    target.OnGetAbility();
                } else {    // Level Up case
                    target.LevelUp();
                }
                if(target.GetLevel+1 < target.maxLevel) {
                    RestoreAbility(target);
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
