using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class Player : LivingEntity, IDamageable {
    static public Player playerInstance;
    public const int DEFAULT_PLAYER_LAYERMASK = 8;

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
    int maxJumpCount = 2;
    int currentJumpCount = 0;
    const int GROUNDABLE_LAYER = 64;
    [SerializeField] BoxCollider2D frontCheckCollider;
    [SerializeField] GameObject groundedPlatform;

    [Header("Dodge Status")]
    float dodgeSpeed = 27f;
    float dodgeDuration = .3f;

    int dodgeCount = 0;
    int maxDodgeCount = 2;
    float cooldownForDodge = 0;
    float dodgeResetTime = 1f;

    #region Coroutines
    Coroutine dodgeCoroutine;
    #endregion

    [Header("Physic Attribute")]
    Rigidbody2D playerRigidbody;
    BoxCollider2D playerCollider;

    [Header("Graphics")]
    SpriteRenderer playerSprite;

    void Awake() {
        if(playerInstance != null)
            Destroy(playerInstance.gameObject);
        playerInstance = this.GetComponent<Player>();

        if(TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponents<BoxCollider2D>()[0];
        frontCheckCollider = frontCheckCollider==null ? GetComponents<BoxCollider2D>()[1] : frontCheckCollider;

        playerSprite = GetComponent<SpriteRenderer>();
    }
    void Start() {
        InitialState();
    }
    void InitialState() {
        floatState.OnActive += () => {
            canMove = false;
            isGrounding = false;
            currentJumpCount ++;
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
        dodgeState.OnActive += () => {
            playerRigidbody.gravityScale = 0;
        };
        dodgeState.OnInactive += () => {
            playerRigidbody.gravityScale = 1;
            if(dodgeCoroutine != null)
                StopCoroutine(dodgeCoroutine);
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
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
            playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            playerStateMachine.ChangeState(floatState, true);
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
        RaycastHit2D[] inners = Physics2D.BoxCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, .02f, GROUNDABLE_LAYER);
        foreach(RaycastHit2D inner in inners) {
            Platform targetPlatform = inner.transform.GetComponent<Platform>();
            if(targetPlatform)
                targetPlatform.DisablePlatform();
        }
    }
    public void Dodge() {
        if(dodgeCount <= 0) return;
        if(dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
        dodgeCoroutine = StartCoroutine(DodgeCoroutine());
    }
    public IEnumerator DodgeCoroutine() {
        playerStateMachine.ChangeState(dodgeState);
        float dirX = moveDirection.x!=0 ? moveDirection.x : transform.localScale.x;
        float offset = 0;
        float v;

        cooldownForDodge = dodgeResetTime;
        dodgeCount --;

        LookAtX(dirX);
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
        ResetDodgeTime();
    }
    protected void ResetDodgeTime() {
        if(cooldownForDodge > 0)
            cooldownForDodge -= Time.deltaTime;
        else if(dodgeCount != maxDodgeCount)
            dodgeCount = maxDodgeCount;
    }
    protected void BasicMove() {
        if(!isGrounding 
        || !CheckFront()
        || playerStateMachine.Compare(dodgeState)) return;

        if(moveDirection == Vector2.zero) { // Stop Moving
            playerStateMachine.ChangeState(idleState, false);
            Vector2 destSpeed = Vector2.Lerp((1 - Time.deltaTime) * playerRigidbody.velocity, playerRigidbody.velocity, .02f);
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
        Bounds b = playerCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(b.center.x, b.center.y - b.size.y/2), new Vector2(b.size.x, .02f), 0, Vector2.down, .01f, GROUNDABLE_LAYER);
        if(hit && !(hit.transform.tag == "Platform" && hit.transform.GetComponent<Platform>().isDeactive)) {
            if(playerRigidbody.velocity.y <= 0) {
                currentJumpCount = 0;
                playerStateMachine.ChangeState(basicState, false);
                groundedPlatform = hit.transform.gameObject;
            }
        } else {
            playerStateMachine.ChangeState(floatState, false);
        }
    }
    
    public int Hp = 100;
    public void OnDamage(int damage) {
        Hp = Hp - damage;
    }
}
