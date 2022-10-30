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
                    Debug.LogWarning($"Player variable is auto-define by {this.name}. If it's not your purpose check player attribute in {this.name}.");
                } else {
                    Debug.LogWarning($"Input manager found 'Player Object' that has not {player.GetType()} script. name:{player.name}");
                }
            } else {
                Debug.LogWarning("There isn't any Player Object in current scene.");
            }
        }
        if(uiManager == null) {
            Debug.LogWarning($"UI Manager in {this.name} is null.");
        }
    }
    
    void Update() {
        float keyDirection = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.Escape)) { // Esc key case

        }
        if(keyDirection == 0) { // any arrow key being not pressed
            player.SetDirection(0);
        } else { // right arrow(<-) or left arrow(->)
            player.SetDirection(keyDirection);
        }
        if(Input.GetButtonDown("Jump")) {
            player.Jump();
        }
        if(Input.GetButtonUp("Jump")) {
            player.StopJump();
        }
        if(Input.GetButtonDown("Down Jump")) {
            player.DownJump();
        }
        if(Input.GetButtonDown("Basic Attack")) {
            player.Attack();
        }
        if(Input.GetButtonDown("Dodge"))
            player.Dodge();
    }
}
