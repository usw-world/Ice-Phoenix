using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class dummy_player : MonoBehaviour {
    State moveState = new State("Move");
    State jumpState = new State("Jump");
    StateMachine playerStateMachine;

    float moveDirectionX;

    Rigidbody2D playerRigidbody;

    void Awake() {
        if(TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(new State("Nothing"));
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = GetComponent<Rigidbody2D>();
    }
    void Start() {
        InitialState();
    }
    void InitialState() {
        jumpState.OnActive += () => {
            playerRigidbody.AddForce(Vector3.up * 100f, ForceMode2D.Force);
        };
        moveState.OnStay += () => {
            // playerRigidbody.AddForce(Vector3.right * moveDirectionX);
            playerRigidbody.velocity = new Vector3(10, 0, 0);
        };
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            playerStateMachine.ChangeState(jumpState);
            playerStateMachine.ChangeState(new State("Nothing")); // very critical code.
        }
        moveDirectionX = Input.GetAxisRaw("Horizontal");
        if(moveDirectionX != 0) {
            playerStateMachine.ChangeState(moveState);
        } else {
            playerStateMachine.ChangeState(new State("Nothing"));
        }
        // if(Input.GetKey(KeyCode.RightArrow)) {
        //     playerStateMachine.ChangeState(moveState);
        // } else {
        //     playerStateMachine.ChangeState(new State("Nothing"));
        // }
    }
}
