using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CmpLinkedList;

namespace AbilitySystem {
    public class AbilityManager : MonoBehaviour {
        static public AbilityManager instance;

        [SerializeField] GameObject levelUpPrefab;

        [SerializeField] Player player;
        [SerializeField] AbilityUI abilityUI;

        [SerializeField] List<Ability> allAbilities = new List<Ability>();
        [SerializeField] List<Ability> abilitiesForEmpty = new List<Ability>();
        IntegratedList<Ability> currentAbilityChoices = new IntegratedList<Ability>();
        IntegratedList<Ability> playersAbilities = new IntegratedList<Ability>();

        private void Awake() {
            if(instance != null) Destroy(this.gameObject);
            else instance = this;

            if(player == null) {
                GameObject pobj = GameObject.FindGameObjectWithTag("Player");
                if(pobj != null)
                    player = pobj.GetComponent<Player>();
            }
            foreach(Ability ability in allAbilities) {
                if(!currentAbilityChoices.Contains(ability))
                    currentAbilityChoices.Push(ability);
            }
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
            Queue<Ability> choices = new Queue<Ability>();
            for(int i=0; i<3; i++) {
                try {
                    index = Random.Range(0, currentAbilityChoices.Count);
                    choices.Enqueue(currentAbilityChoices.Shift(index));
                } catch(EmptyReferenceException ere) {
                    index = Random.Range(0, abilitiesForEmpty.Count);
                    choices.Enqueue(abilitiesForEmpty[index]);
                    // throw new System.NotImplementedException("특성이 모두 바닥나서 플레이어한테 보여줄 특성이 없습니다. 이제 전 어쩌죠.");
                }
            }
            for(int i=0; i<3; i++) {
                Ability choice = choices.Dequeue();
                currentAbilityChoices.Push(choice);
                abilityUI.SetChoice(i, choice);
            }
            abilityUI.ShowChoices();
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
        }
        public void RemoveAbility(Ability ability) {
            ability.OnReleaseAbility();
            playersAbilities.Remove(ability);
        }
        public void RestoreAbility(Ability ability) {
            currentAbilityChoices.Push(ability);
        }
        public void AbilityLevelUp(Ability ability) {
            ability.LevelUp();
        }
    }
}
