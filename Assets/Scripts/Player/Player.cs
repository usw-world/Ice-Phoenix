using System.Collections;
using UnityEngine;
using GameObjectState;
using JetBrains.Annotations; // << ?
using UnityEngine.UI;

public class Player : LivingEntity, IDamageable {
    static public Player playerInstance;
    public const int DEFAULT_PLAYER_LAYERMASK = 8;
    public Animator playerAni;

    protected State idleState = new State("Idle");
    protected State moveState = new State("Move");
    protected State floatState = new State("Float");
    protected State dodgeState = new State("Dodge");
    protected State attackState = new State("Attack");
    protected State hitState = new State("Hit");

    protected State basicState { get {
        if(moveDirection == Vector2.zero) return idleState;
        else return moveState;
    } }
    protected StateMachine playerStateMachine;

    [Header("Move Status")]
    float moveSpeed = 10f;
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
    float dodgeSpeed = 33f;
    float dodgeDuration = .3f;

    int dodgeCount = 0;
    int maxDodgeCount = 2;
    float cooldownForDodge = 0;
    float dodgeResetTime = 1f;

    #region Coroutines
    protected Coroutine dodgeCoroutine;
    protected Coroutine hitCoroutine;
    protected Coroutine basicAttackCoroutine;
    #endregion

    [Header("Physic Attribute")]
    protected Rigidbody2D playerRigidbody;
    protected BoxCollider2D playerCollider;

    [Header("Graphics")]
    protected SpriteRenderer playerSprite;
    protected Animator playerAnimator;
    protected Color playerOriginColor;

    [Header("UI")]
    [SerializeField] PlayerSideUI playerSideUI;
    
    protected override void Awake() {
        base.Awake();
        if (Player.playerInstance != null)
            Destroy(Player.playerInstance.gameObject);
        Player.playerInstance = this.GetComponent<Player>();

        if (TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponents<BoxCollider2D>()[0];
        frontCheckCollider = frontCheckCollider==null ? GetComponents<BoxCollider2D>()[1] : frontCheckCollider;

        playerSprite = GetComponent<SpriteRenderer>();
        playerOriginColor = playerSprite==null ? Color.white : playerSprite.color;
        playerAnimator = playerAnimator==null ? GetComponent<Animator>() : playerAnimator;

        playerSideUI = playerSideUI==null ? GetComponentInChildren<PlayerSideUI>() : playerSideUI;
    }
    protected override void Start() {
        InitialState();
        RefreshHPSlider();
    }
    protected virtual void InitialState() {
        #region Idle State >>
        idleState.OnActive += () => {
            playerAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive += () => {
            playerAnimator.SetBool("Idle", false);
        };
        #endregion << Idle State

        #region Float State >>
        floatState.OnActive += () => {
            canMove = false;
            isGrounding = false;
            playerAnimator.SetBool("Float", true);
            currentJumpCount ++;
        };
        floatState.OnInactive += () => {
            canMove = true;
            isGrounding = true;
            playerAnimator.SetBool("Float", false);
        };
        floatState.OnStay += () => {
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .05f);
            LookAtX(moveDirection.x);
            playerRigidbody.velocity = addingSpeed;
        };
        #endregion << Float State

        #region Move State >>
        moveState.OnActive += () => {
            playerAnimator.SetBool("Move", true);
        };
        moveState.OnStay += () => {
            LookAtX(moveDirection.x);
        };
        moveState.OnInactive += () => {
            playerAnimator.SetBool("Move", false);
        };
        #endregion << Move State

        #region Dodge State >>
        dodgeState.OnActive += () => {
            playerRigidbody.gravityScale = 0;
            playerAnimator.SetBool("Dodge", true);
        };
        dodgeState.OnInactive += () => {
            playerRigidbody.gravityScale = 1;
            playerAnimator.SetBool("Dodge", false);
            if(dodgeCoroutine != null)
                StopCoroutine(dodgeCoroutine);
        };
        #endregion << Dodge State
        
        #region Hit State >>
        hitState.OnActive += () => {
            canMove = false;
            playerAnimator.SetBool("Hit", true);
        };
        hitState.OnInactive += () => {
            canMove = true;
            playerAnimator.SetBool("Hit", false);
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
        };
        #endregion << Hit State
    }
    public void SetDirection(float dirX) {
        moveDirection = Vector2.right * dirX;
    }
    private bool CheckFront() {
        RaycastHit2D hit = Physics2D.BoxCast(frontCheckCollider.bounds.center, frontCheckCollider.bounds.size, 0, transform.forward, .02f, GROUNDABLE_LAYER);
        return !hit;
    }
    public void Jump() {
        if(currentJumpCount < maxJumpCount
        && !playerStateMachine.Compare(dodgeState)
        && !playerStateMachine.Compare(hitState)) {
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
        if(dodgeCount <= 0
        || playerStateMachine.Compare(hitState)) return;
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
    public virtual void BasicAttack() {}
    protected virtual void Update() {
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
        || playerStateMachine.Compare(floatState)
        || playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(attackState)) return;

        if(moveDirection == Vector2.zero) { // Stop Moving
            playerStateMachine.ChangeState(idleState, false);
            // Vector2 destSpeed = Vector2.Lerp((1 - Time.deltaTime) * playerRigidbody.velocity, playerRigidbody.velocity, .02f);
            // playerRigidbody.velocity = destSpeed;
            playerRigidbody.velocity *= Vector2.up;
        } else { // Stay Running
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .15f);
            playerRigidbody.velocity = addingSpeed;
            playerStateMachine.ChangeState(moveState, false);
        }
    }
    protected void CheckBottom() {
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(attackState))
            return;
        Bounds b = playerCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(b.center.x, b.center.y - b.size.y/2), new Vector2(b.size.x, .02f), 0, Vector2.down, .01f, GROUNDABLE_LAYER);
        if(hit && !(hit.transform.tag == "Platform" && hit.transform.GetComponent<Platform>().isInactive)) {
            if(playerRigidbody.velocity.y <= 0) {
                currentJumpCount = 0;
                playerStateMachine.ChangeState(basicState, false);
                groundedPlatform = hit.transform.gameObject;
            }
        } else {
            playerStateMachine.ChangeState(floatState, false);
        }
    }
    
    void RefreshHPSlider() {
        if(maxHp != 0)
            playerSideUI.SetHPSlider(hp / maxHp);
    }
    public void OnDamage(float damage, float duration=.25f) {
        if(isDead) return;
        hp -= damage;
        RefreshHPSlider();
        if(hp <= 0) {
            Die();
        } else {
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(duration));
        }
    }
    public void OnDamage(float damage, Vector2 force, float duration=.25f) {
        if(isDead) return;
        OnDamage(damage, duration);
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(force, ForceMode2D.Force);
    }
    protected override void Die() {
        base.Die();
    }
    IEnumerator HitCoroutine(float duration) {
        if(duration > 0) {
            playerStateMachine.ChangeState(hitState);
            yield return new WaitForSeconds(duration);
            playerStateMachine.ChangeState(basicState);
        }
    }
}
