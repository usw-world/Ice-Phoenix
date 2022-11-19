using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem {
    public class AbilityManager : MonoBehaviour {
        [SerializeField] Canvas abilityUICanvas;
        [SerializeField] Player player;

        List<Ability> abilities = new List<Ability>();

        private void Awake() {
            if(player == null) {
                GameObject pobj = GameObject.FindGameObjectWithTag("Player");
                if(pobj != null)
                    player = pobj.GetComponent<Player>();
            }
        }
        public void AddAbility(Ability ability) {
            ability.OnGetAbility();
            abilities.Add(ability);
        }
        public void RemoveAbility(Ability ability) {
            ability.OnReleaseAbility();
            abilities.Remove(ability);
        }
    }
}
