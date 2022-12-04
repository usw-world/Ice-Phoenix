using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class InputManager : MonoBehaviour {
    static public InputManager instance;

    private StateMachine inputStateMachine;
    public State playState { get; private set; } = new State("Play");
    public State menuState { get; private set; } = new State("Menu");

    [SerializeField] Player player;
    [SerializeField] GameObject playerObject;
    
    void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        inputStateMachine = GetComponent<StateMachine>();
        inputStateMachine.SetIntialState(playState);
    }
    void Start() {
        InitialState();
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
    }
    private void InitialState() {
        // menuState.OnActive += (State prevState) => {
        //     UIManager.instance.OpenUI(UIManager.instance.escapeMenu);
        // };
        // menuState.OnInactive += (State prevState) => {
        //     UIManager.instance.CloseUI();
        // };
    }
    
    void Update() {
        if(inputStateMachine.Compare(playState))
            PlayInputSet();
            
        else if(inputStateMachine.Compare(menuState))
            MenuInputSet();
    }
    public void SetInputState(State next) {
        inputStateMachine.ChangeState(next);
    }
    private void PlayInputSet() {
        float keyDirection = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.Escape)) { // Esc key case
            UIManager.instance.OpenUI(UIManager.instance.escapeMenu);
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
        if(Input.GetKeyDown(KeyCode.Tab))
            UIManager.instance.TogglePlayerStatusUI();
    }
    private void MenuInputSet() {
        UIManager.instance.activingUI.KeyPressEvent();
        // if(Input.GetKeyDown(KeyCode.Escape))
        //     UIManager.instance.CloseUI();
    }
}
