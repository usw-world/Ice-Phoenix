using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ability {
    public class AbilityManager : MonoBehaviour {
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
