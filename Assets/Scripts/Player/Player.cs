using System;
using System.Collections;
using UnityEngine;
using GameObjectState;
using UnityEngine.UI;
using AbilitySystem;

public class Player : LivingEntity, IDamageable {
    public const int DEFAULT_PLAYER_LAYERMASK = 8;
    public const string ATTACK_STATE_TAG = "tag:Attack";
    public const string JUMP_ATTACK_STATE_TAG = "tag:Jump Attack";

    static public Player playerInstance;
    public delegate float Coefficients();

    #region States (and State Machine)
    protected StateMachine playerStateMachine;
    protected State idleState = new State("Idle");
    protected State moveState = new State("Move");
    protected State floatState = new State("Float");
    protected State dodgeState = new State("Dodge");
    protected State hitState = new State("Hit");
    protected State basicState { get {
        if(moveDirection == Vector2.zero) return idleState;
        else return moveState;
    } }
    #endregion
    #region Move
    [Header("Move Status")]
    public Coefficients moveSpeedAttribute;
    protected float defaultMoveSpeed = 10f;
    protected float moveSpeedCoef {
        get {
            float coef = 1;
            coef += adaptationManager.points[(int)Adaptation.Type.Movement] * .015f;
            if(moveSpeedAttribute != null) {
                Delegate[] coefficients = moveSpeedAttribute.GetInvocationList();
                for (int i=0; i<coefficients.Length; i++) {
                    coef += ((Coefficients) coefficients[i])();
                }
            }
            return coef;
        }
    }
    protected float moveSpeed {
        get { return defaultMoveSpeed * moveSpeedCoef; }
    }
    protected float jumpPower = 22f;
    protected Vector2 moveDirection;
    protected bool canMove = true;
    bool isGrounding = false;
    int maxJumpCount = 2;
    int currentJumpCount = 0;
    const int GROUNDABLE_LAYER = 64;
    [SerializeField] GameObject groundedPlatform;
    #endregion Move
    #region Attack
    private float defaultDamage = 10f;
    public Coefficients damageCoefs;
    protected float attackDamage {
        get {
            float coef = 1;
            int power = adaptationManager.points[(int)Adaptation.Type.Power];
            coef += power * .02f;
            if(damageCoefs != null) {
                Delegate[] coefficients = damageCoefs.GetInvocationList();
                for(int i=0; i<coefficients.Length; i++) {
                    coef += ((Coefficients) coefficients[i])();
                }
            }
            return defaultDamage * coef;
        }
    }
    protected float attackForce = 100f;
    protected float defaultAttackSpeed = 1f;
    public Coefficients attackSpeedCoefs;
    protected float attackSpeed {
        get {
            float coef = 1;
            int fast = adaptationManager.points[(int)Adaptation.Type.Fast];
            coef += fast * .02f;
            if(attackSpeedCoefs != null) { 
                Delegate[] coefficients = attackSpeedCoefs.GetInvocationList();
                for(int i=0; i<coefficients.Length; i++) {
                    coef += ((Coefficients) coefficients[i])();
                }
            }
            return defaultAttackSpeed * coef;
        }
    }
    protected bool isAfterAttack = false;
    #endregion Attack
    #region Defence
    float defaultArmor = 1;
    public Coefficients armorCoefficients;
    float armor {
        get {
            float coef = 0;
            int strong = adaptationManager.points[(int)Adaptation.Type.Strong];
            Delegate[] coefficient = armorCoefficients.GetInvocationList();
            for(int i=0; i<coefficient.Length; i++) {
                coef += ((Coefficients) coefficient[i])();
            }
            return (defaultArmor * coef) + (strong * .01f);
        }
    }
    #endregion Defence
    #region Dodge
    [Header("Dodge Status")]
    float dodgeSpeed = 33f;
    float dodgeDuration = .3f;
    float dodgeCoef {
        get {
            float coef = 1;
            float movement = adaptationManager.points[(int)Adaptation.Type.Movement];
            return coef + (movement * 0.01f);
        }
    }
    int dodgeCount = 0;
    int maxDodgeCount = 2;
    float cooldownForDodge = 0;
    float dodgeResetTime = 1f;
    #endregion Dodge
    #region Coroutines
    protected Coroutine dodgeCoroutine;
    protected Coroutine hitCoroutine;
    #endregion
    #region Physics
    [Header("Physic Attribute")]
    protected Rigidbody2D playerRigidbody;
    protected BoxCollider2D playerCollider;
    #endregion Physics
    #region Graphics
    [Header("Graphics")]
    protected SpriteRenderer playerSprite;
    protected Animator playerAnimator;
    protected Color playerOriginColor;
    #endregion Graphics
    #region UI
    [Header("UI")]
    [SerializeField] SideUI playerSideUI;
    #endregion UI
    #region ActionEvent
    public delegate void DodgeEvent(Vector2 direction);
    #endregion ActionEvent
    #region Ability
    public float abilityCoef {
        get { return 1 + (adaptationManager.points[(int)Adaptation.Type.Ability] * 0.03f); }
    }
    [SerializeField] protected AbilityManager abilityManager;
    #endregion Ability
    #region Adaptation
    [SerializeField] protected Adaptation adaptationManager;
    #endregion Adaptation

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

        playerSprite = GetComponent<SpriteRenderer>();
        playerOriginColor = playerSprite==null ? Color.white : playerSprite.color;
        playerAnimator = playerAnimator==null ? GetComponent<Animator>() : playerAnimator;

        playerSideUI = playerSideUI==null ? GetComponentInChildren<SideUI>() : playerSideUI;
        
        abilityManager = abilityManager==null ? GetComponentInChildren<AbilityManager>() : abilityManager;

        adaptationManager = adaptationManager==null ? GetComponentInChildren<Adaptation>() : adaptationManager;
    }
    protected override void Start() {
        InitialState();
        playerSideUI.UpdateHPSlider(this);
    }
    protected virtual void InitialState() {
        #region Idle State >>
        idleState.OnActive += (prevState) => {
            playerAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive += (nextState) => {
            playerAnimator.SetBool("Idle", false);
        };
        #endregion << Idle State

        #region Float State >>
        floatState.OnActive += (prevState) => {
            isGrounding = false;
            if(prevState.Compare(floatState)) playerAnimator.SetTrigger("Double Jump");
            else playerAnimator.SetBool("Float", true);
            currentJumpCount ++;
        };
        floatState.OnInactive += (nextState) => {
            isGrounding = true;
            playerAnimator.SetBool("Float", false);
        };
        floatState.OnStay += () => {
            LookAtX(moveDirection.x);
            if(!CheckFront()) return;
            Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
            Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .05f);
            playerRigidbody.velocity = addingSpeed;
        };
        #endregion << Float State

        #region Move State >>
        moveState.OnActive += (prevState) => {
            playerAnimator.SetBool("Move", true);
            playerAnimator.speed = moveSpeedCoef;
        };
        moveState.OnInactive += (nextState) => {
            playerAnimator.SetBool("Move", false);
            playerAnimator.speed = 1;
        };
        #endregion << Move State

        #region Dodge State >>
        dodgeState.OnActive += (prevState) => {
            playerRigidbody.gravityScale = 0;
            playerAnimator.SetBool("Dodge", true);
        };
        dodgeState.OnInactive += (nextState) => {
            playerRigidbody.gravityScale = 1;
            playerAnimator.SetBool("Dodge", false);
            if(dodgeCoroutine != null)
                StopCoroutine(dodgeCoroutine);
        };
        #endregion << Dodge State
        
        #region Hit State >>
        hitState.OnActive += (prevState) => {
            playerAnimator.SetBool("Hit", true);
        };
        hitState.OnInactive += (nextState) => {
            playerAnimator.SetBool("Hit", false);
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
        };
        #endregion << Hit State
    }
    public void SetDirection(float dirX) {
        moveDirection = Vector2.right * dirX;
    }
    protected bool CheckFront() {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0, new Vector2(moveDirection.x, 0), .02f, GROUNDABLE_LAYER);
        for(int i=0; i<hit.Length; i++) {
            Platform p = hit[i].transform.GetComponent<Platform>();
            if(hit[i].transform.tag == "Ground") {
                return false;
            }
        }
        return true; 
    }
    public void Jump() {
        if(currentJumpCount >= maxJumpCount
        || playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(ATTACK_STATE_TAG) && !canMove)
            return;
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
        playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        playerStateMachine.ChangeState(floatState, true);
    }
    public void StopJump() {
        if(playerRigidbody.velocity.y > 0) {
            Vector2 nextV = playerRigidbody.velocity;
            nextV.y /= 2;
            playerRigidbody.velocity = nextV;
        }
    }
    public void DownJump() {
        if(playerStateMachine.Compare(ATTACK_STATE_TAG)) return;
        
        RaycastHit2D[] inners = Physics2D.BoxCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, .02f, GROUNDABLE_LAYER);
        foreach(RaycastHit2D inner in inners) {
            Platform targetPlatform = inner.transform.GetComponent<Platform>();
            if(targetPlatform)
                targetPlatform.DisablePlatform();
        }
    }
    public void Dodge() {
        if(dodgeCount <= 0
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG)) return;
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
        while(offset < 1f) {
            v = Mathf.Lerp(dodgeSpeed * dodgeCoef * dirX, 0, offset);
            playerRigidbody.velocity = new Vector2(v, playerRigidbody.velocity.y);
            offset += Time.deltaTime/dodgeDuration*dodgeCoef;
            yield return 0;
        }
        offset = 1f;
        v = Mathf.Lerp(dodgeSpeed * dodgeCoef * dirX, 0, offset);
        playerRigidbody.velocity = new Vector2(v, playerRigidbody.velocity.y);

        playerStateMachine.ChangeState(basicState);
    }
    public virtual void Attack() {}
    public virtual void JumpAttack() {}
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
        || playerStateMachine.Compare(floatState)
        || playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        || playerStateMachine.Compare(ATTACK_STATE_TAG) && !canMove
        || playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG)) return;

        if(moveDirection == Vector2.zero) { // Stop Moving
            if(playerStateMachine.Compare(ATTACK_STATE_TAG)) return; // This code keep 'delay motion after attack' when Player hasn't movement intention.
            playerStateMachine.ChangeState(idleState, false);
            // ┌━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┐
            // │ Vector2 destSpeed = Vector2.Lerp((1 - Time.deltaTime) * playerRigidbody.velocity, playerRigidbody.velocity, .02f); │
            // │ playerRigidbody.velocity = destSpeed;                                                                              ┣ Inertia Movement
            // └━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┘
            playerRigidbody.velocity *= Vector2.up;
        } else { // Stay Running
            LookAtX(moveDirection.x);
            if(CheckFront()) {
                Vector2 maxSpeed = (moveSpeed * moveDirection) + new Vector2(0, playerRigidbody.velocity.y);
                Vector2 addingSpeed = Vector2.Lerp(playerRigidbody.velocity, maxSpeed, .15f);
                playerRigidbody.velocity = addingSpeed;
                playerStateMachine.ChangeState(moveState, false);
            }
        }
    }
    protected void CheckBottom() {
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        /* || playerStateMachine.Compare(ATTACK_STATE_TAG) */)
            return;
        Bounds b = playerCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(b.center.x, b.center.y - b.size.y/2), new Vector2(b.size.x, .02f), 0, Vector2.down, .01f, GROUNDABLE_LAYER);
        
        if(hit && !(hit.transform.tag == "Platform" && hit.collider.bounds.center.y + hit.collider.bounds.size.y/2 >= hit.point.y)) {
            if(playerRigidbody.velocity.y <= 0) {
                if(playerStateMachine.Compare(ATTACK_STATE_TAG)) return;
                currentJumpCount = 0;
                playerStateMachine.ChangeState(basicState, false);
                groundedPlatform = hit.transform.gameObject;
            }
        } else {
            if(!playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG))
                playerStateMachine.ChangeState(floatState, false);
        }
    }
    protected override float SetHP(float next){
        float nextHp = base.SetHP(next);
        playerSideUI.UpdateHPSlider(this);
        return nextHp;
    }
    private float IncreasHP(float amount) {
        float nextHp = SetHP(hp + amount);
        return hp;
    }
    public void OnDamage(float damage, float duration=.25f) {
        if(isDead) return;
        IncreasHP(-damage);
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
