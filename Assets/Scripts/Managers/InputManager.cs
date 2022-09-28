using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    [SerializeField] Player player;
    [SerializeField] GameObject playerObject;

    [SerializeField] UIManager uiManager;
    
    void Start() {
        if(player == null) {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if(playerObject != null) {
                if(playerObject.TryGetComponent<Player>(out player)) {
                    Debug.LogWarning($"Input manager found 'Player Object' that has not {player.GetType()} script.");
                } else {
                    Debug.LogWarning($"Player variable is auto-define by {this.name}. If it's not your purpose check player attribute in {this.name}.");
                }
            } else {
                Debug.LogWarning("There isn't any Player Object in current scene.");
            }
        }
        if(uiManager == null) {
            Debug.LogError($"UI Manager in {this.name} is null.");
        }
    }
    
    void Update() {
        
    }
}
