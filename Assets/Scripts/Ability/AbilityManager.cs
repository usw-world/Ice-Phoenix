using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem {
    public class AbilityManager : MonoBehaviour {
        [SerializeField] Canvas abilityUICanvas;
        Player player;
        private void Start() {
            if(player == null) {
                Debug.LogWarning($"Player is null in {this.name} within {this.gameObject.name}.");
                GameObject pobj = GameObject.FindGameObjectWithTag("Player");
                if(pobj != null)
                    player = pobj.GetComponent<Player>();
            }
        }
    }
}
