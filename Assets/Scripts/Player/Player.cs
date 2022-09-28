using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class Player : MonoBehaviour {
    State idleState = new State("Idle");
    State moveState = new State("Move");
    State jumpState = new State("Jump");
    StateMachine playerStateMachine;

    [Header("Move Status")]
    float moveSpeed = 10f;
    float jumpPower = 22f;
    Vector2 moveDirection;
    bool canMove = true;
    int maxJumpCount = 1;
    int currentJumpCount = 0;
    const int GROUNDABLE_LAYER = 64;

    [Header("Physic Attribute")]
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    void Awake() {
        if(TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(new State("Nothing"));
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        if(playerCollider is not BoxCollider2D)
            print("I think it's good what player collider is BoxCollider2D. But not now.");
    }
    void Start() {
        InitialState();
    }
    void InitialState() {
        jumpState.OnActive += () => {
            playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        };
        moveState.OnStay += () => {
            playerRigidbody.velocity = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
        };
        idleState.OnActive += () => {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
        };
    }
    public void SetDirection(float dirX) {
        if(dirX == 0) {
            playerStateMachine.ChangeState(idleState);
        } else {
            if(!canMove) return;
            moveDirection = Vector2.right * dirX;
            playerStateMachine.ChangeState(moveState);
        }
    }
    private void CheckFront() {
        
    }
    public void Jump() {
        if(currentJumpCount < maxJumpCount) {
            playerStateMachine.ChangeState(jumpState);
            currentJumpCount ++;
        }
    }
    public void DownJump() {
        
    }
    public void Dodge() {

    }
    void Update() {
        CheckBottom();
    }
    private void OnCollisionEnter2D(Collision2D collisionInfo) {
        if(1<<collisionInfo.collider.gameObject.layer == GROUNDABLE_LAYER) {
            RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, playerCollider.bounds.size.y/2 + .02f, GROUNDABLE_LAYER);
            if(hit) {
                currentJumpCount = 0;
                playerStateMachine.ChangeState(idleState);
            }
        }
    }
    protected void CheckBottom() {
    }
}
