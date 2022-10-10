using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class Player : LivingEntity, IDamageable {
    protected State idleState = new State("Idle");
    protected State moveState = new State("Move");
    protected State floatState = new State("Float");
    protected State dodgeState = new State("Dodge");
    protected State basicState { get {
        if(moveDirection == Vector2.zero) return idleState;
        else return moveState;
    } }
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

    [Header("Dodge Status")]
    float dodgeSpeed = 30f;
    float dodgeDuration = .4f;
    float doubletabDir = 0;

    int dodgeCount = 0;
    int maxDodgeCount = 2;
    float cooldownForDodge = 0;
    float dodgeResetTime = 1f;

    #region Coroutines
    Coroutine dodgeCoroutine;
    #endregion

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
            LookAtX(moveDirection.x);
            playerRigidbody.velocity = addingSpeed;
        };
        moveState.OnStay += () => {
            LookAtX(moveDirection.x);
        };
        dodgeState.OnInactive += () => {
            if(dodgeCoroutine != null)
                StopCoroutine(dodgeCoroutine);
        };
    }
    public void SetDirection(float dirX) {
        moveDirection = Vector2.right * dirX;
    }
    public void SetDoubleTab(float dirX) {
        if(doubletabDir < 0 && dirX < 0
        || doubletabDir > 0 && dirX > 0)
            Dodge();
        else
            doubletabDir = dirX * .2f;
    }
    public void ResetDoubleTab() {
        if(doubletabDir > 0)
            doubletabDir = Mathf.Max(0, doubletabDir - Time.deltaTime);
        else
            doubletabDir = Mathf.Min(0, doubletabDir + Time.deltaTime);
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
        if(dodgeCount <= 0) return;
        if(dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
        dodgeCoroutine = StartCoroutine(DodgeCoroutine());
    }
    public IEnumerator DodgeCoroutine() {
        playerStateMachine.ChangeState(dodgeState);
        doubletabDir = 0;
        float dirX = moveDirection.x==0 ? transform.localScale.x : moveDirection.x;
        float offset = 0;
        float v;

        cooldownForDodge = dodgeResetTime;
        dodgeCount --;
        playerRigidbody.velocity = Vector2.zero;
        while(offset < 1) {
            v = Mathf.Lerp(dodgeSpeed * dirX, 0, offset);
            playerRigidbody.velocity = new Vector2(v, playerRigidbody.velocity.y);
            offset += Time.deltaTime/dodgeDuration;
            yield return 0;
        }
        offset = 1f;
        v = Mathf.Lerp(dodgeSpeed * dirX, 0, offset);
        playerRigidbody.velocity = new Vector2(v, playerRigidbody.velocity.y);

        playerStateMachine.ChangeState(basicState);
    }
    public virtual void BasicAttack(){}
    void Update() {
        BasicMove();
        CheckBottom();
        ResetDoubleTab();
        ResetDodgeTime();
    }
    protected void ResetDodgeTime() {
        if(dodgeResetTime > 0)
            dodgeResetTime -= Time.deltaTime;
        else if(dodgeCount != maxDodgeCount)
            dodgeCount = maxDodgeCount;
    }
    protected void BasicMove() {
        if(!isGrounding 
        || !CheckFront()
        || playerStateMachine.Compare(dodgeState)) return;

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
    protected void LookAtX(float x) {
        if(x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
    protected void CheckBottom() {
        if(playerStateMachine.Compare(dodgeState))
            return;

        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size * .99f, 0, Vector2.down, .02f, GROUNDABLE_LAYER);
        if(hit) {
            if(playerRigidbody.velocity.y <= 0) {
                currentJumpCount = 0;
                playerStateMachine.ChangeState(basicState, false);
                groundedPlatform = hit.transform.gameObject;
            }
        } else {
            playerStateMachine.ChangeState(floatState);
        }
    }
    
    public int Hp = 100;
    public void OnDamage(int damage) {
        Hp = Hp - damage;
    }
}
