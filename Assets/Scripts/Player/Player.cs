using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class Player : LivingEntity, IDamageable {
    State idleState = new State("Idle");
    State moveState = new State("Move");
    State floatState = new State("Float");
    State dodgeState = new State("Dodge");
    StateMachine playerStateMachine;

    [Header("Move Status")]
    float moveSpeed = 9f;
    float jumpPower = 25f;
    Vector2 moveDirection;
    bool canMove = true;
    bool isGrounding = false;
    int maxJumpCount = 1;
    int currentJumpCount = 0;
    const int GROUNDABLE_LAYER = 64;
    [SerializeField] BoxCollider2D frontCheckCollider;
    [SerializeField] GameObject groundedPlatform;

    [Header("Physic Attribute")]
    Rigidbody2D playerRigidbody;
    CapsuleCollider2D playerCollider;

    void Awake() {
        if(TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(new State("Nothing"));
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        frontCheckCollider = frontCheckCollider==null ? GetComponent<BoxCollider2D>() : frontCheckCollider;
    }
    void Start() {
        InitialState();
    }
    void InitialState() {
        floatState.OnActive += () => {
            canMove = false;
            isGrounding = false;
        };
        floatState.OnInactive += () => {
            canMove = true;
            isGrounding = true;
        };
        floatState.OnStay += () => {
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .05f);
            playerRigidbody.velocity = addingSpeed;
        };
    }
    public void SetDirection(float dirX) {
        moveDirection = Vector2.right * dirX;
    }
    private bool CheckFront() {
        RaycastHit2D hit = Physics2D.BoxCast(frontCheckCollider.bounds.center, frontCheckCollider.bounds.size, 0, transform.forward, .02f, GROUNDABLE_LAYER);
        return !hit;
    }
    public void Jump() {
        if(currentJumpCount < maxJumpCount) {
            currentJumpCount ++;
            playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            playerStateMachine.ChangeState(floatState);
        }
    }
    public void StopJump() {
        if(playerRigidbody.velocity.y > 0) {
            Vector2 nextV = playerRigidbody.velocity;
            nextV.y /= 2;
            playerRigidbody.velocity = nextV;
        }
    }
    public void DownJump() {
        
    }
    public void Dodge() {

    }
    public virtual void BasicAttack(){}
    void Update() {
        BasicMove();
        CheckBottom();
    }
    protected void BasicMove() {
        if(!isGrounding && !CheckFront()) return;

        if(moveDirection == Vector2.zero) { // Stop Moving
            playerStateMachine.ChangeState(idleState, false);
            Vector2 destSpeed = Vector2.Lerp((1 - Time.deltaTime) * playerRigidbody.velocity, playerRigidbody.velocity, .2f);
            playerRigidbody.velocity = destSpeed;
        } else { // Stay Running
            if(!canMove) return;
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .15f);
            playerRigidbody.velocity = addingSpeed;
            playerStateMachine.ChangeState(moveState, false);
        }
    }
    protected void CheckBottom() {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size * .99f, 0, Vector2.down, .02f, GROUNDABLE_LAYER);
        if(hit) {
            if(playerRigidbody.velocity.y <= 0) {
                currentJumpCount = 0;
                playerStateMachine.ChangeState(idleState, false);
                groundedPlatform = hit.transform.gameObject;
            }
        } else {
            playerStateMachine.ChangeState(floatState);
        }
    }
    public void OnDamage() {
        print("player is hit damage.");
    }
}
