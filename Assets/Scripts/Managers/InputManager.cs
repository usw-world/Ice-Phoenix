using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class InputManager : MonoBehaviour {
    static public InputManager instance;

    private StateMachine inputStateMachine;
    public State playState { get; private set; } = new State("Play");
    public State menuState { get; private set; } = new State("Menu");

    Player player;
    
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
        player = Player.playerInstance;
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
            UIManager.instance.OpenUI(UIManager.instance.escapeMenuUI);
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
        player.SetDirection(0);
        UIManager.instance.activingUI.KeyPressEvent();
        // if(Input.GetKeyDown(KeyCode.Escape))
        //     UIManager.instance.CloseUI();
    }
}
