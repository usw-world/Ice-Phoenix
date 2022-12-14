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

    [SerializeField] public GameObject normalCamera;
    [SerializeField] public GameObject zoomCamera;

    static public Player playerInstance;
    public delegate float Coefficients();
    public delegate void DamageEnemyEvent(Transform target);

    #region States (and State Machine)
    protected StateMachine playerStateMachine;
    public State idleState { get; protected set; } = new State("Idle");
    public State moveState { get; protected set; } = new State("Move");
    public State floatState { get; protected set; } = new State("Float");
    public State dodgeState { get; protected set; } = new State("Dodge");
    public State hitState { get; protected set; } = new State("Hit");
    public State basicState{ get {
        if(moveDirection == Vector2.zero) return idleState;
        else return moveState;
    } }
    public State currentState { get { return playerStateMachine.currentState; } }
    #endregion
    #region Move
    [Header("Move Status")]
    public Coefficients moveSpeedAttribute;
    protected float defaultMoveSpeed = 10f;
    protected float moveSpeedCoef {
        get {
            float coef = 1;
            coef += adaptationManager.points[(int)AdaptationManager.Type.Movement] * .015f;
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
    int maxJumpCount = 2;
    int currentJumpCount = 0;
    const int GROUNDABLE_LAYER = 64;
    #endregion Move
    #region Attack
    private float defaultDamage = 10f;
    public Coefficients damageCoefs;
    protected float attackDamage {
        get {
            float coef = 1;
            int power = adaptationManager.points[(int)AdaptationManager.Type.Power];
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
            int fast = adaptationManager.points[(int)AdaptationManager.Type.Fast];
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
    public Coefficients criticalChanceCoef;
    public float criticalChance {
        get {
            float coef = 0;;
            if(criticalChanceCoef != null) {
                Delegate[] coefs = criticalChanceCoef.GetInvocationList();
                for(int i=0; i<coefs.Length; i++) {
                    coef += ((Coefficients)coefs[i])();
                }
            }
            return coef;
        }
    }
    public DamageEnemyEvent basicAttackDamageEvent;
    public DamageEnemyEvent jumpAttackDamageEvent;
    protected bool isAfterAttack = false;
    public float abilityCoef {
        get { return 1 + (adaptationManager.points[(int)AdaptationManager.Type.Ability] * 0.03f); }
    }
    #endregion Attack
    #region Defence
    float defaultArmor = 1;
    public Coefficients armorCoefficients;
    float armor {
        get {
            float coef = 0;
            int strong = adaptationManager.points[(int)AdaptationManager.Type.Strong];
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
            float movement = adaptationManager.points[(int)AdaptationManager.Type.Movement];
            return coef + (movement * 0.01f);
        }
    }
    int dodgeCount = 0;
    int maxDodgeCount = 2;
    float cooldownForDodge = 0;
    float dodgeResetTime = 1f;
    [SerializeField] ParticleSystem dodgeParticle;
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
    // #region UI
    // [Header("UI")]
    // #endregion UI
    #region ActionEvent
    public delegate void DodgeEvent(Vector2 direction);
    #endregion ActionEvent
    #region Sound
    [Header("Sound Clips")]
    [SerializeField] protected SoundPlayer playerSoundPlayer;
    [SerializeField] protected AudioClip[] playerFootstepClip;
    [SerializeField] protected AudioClip playerHitClip;
    [SerializeField] protected AudioClip playerJumpClip;
    [SerializeField] protected AudioClip playerLandClip;
    [SerializeField] protected AudioClip playerDodgeClip;
    [SerializeField] protected AudioClip playerDieClip;
    #endregion Sound
    #region Adaptation
    public int rate { get; protected set; } = 0;
    public int rateGauge { get; protected set; } = 0;
    public int nextRateGauge { get; protected set; } = 150;
    [SerializeField] protected AdaptationManager adaptationManager;
    #endregion Adaptation
    #region Particles
    GameObject particleAttack;
    #endregion Particles
    #region Level (Experience)
    [Header("Level")]
    [SerializeField] protected Slider expSlider;
    public int playerLevel { get; protected set; } = 1;
    public int nextLevelExp { get; protected set; } = 100;
    public int currentExp { get; protected set; } = 0;
    [SerializeField] Material expParticleMaterial;
    delegate void IncreaseIntEvent(int amount);
    IncreaseIntEvent increaseExpEvent;
    Coroutine expSmoothIncreaseCoroutine;
    int abilityPoint = 0;
    #endregion Level (Experience)
    #region Die
    [SerializeField] GameObject playerDieParticle;
    #endregion Die

    protected override void Awake() {
        base.Awake();
        if (Player.playerInstance == null) {
            Player.playerInstance = this;
            DontDestroyOnLoad(this.gameObject);
            GameManager.instance.destroyObjectsOnGameOver.Add(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }

        if (TryGetComponent<StateMachine>(out playerStateMachine)) {
            playerStateMachine.SetIntialState(idleState);
        } else {
            Debug.LogError("Player hasn't any 'StateMachine'.");
        }
        playerRigidbody = playerRigidbody==null ? GetComponent<Rigidbody2D>() : playerRigidbody;
        playerCollider = playerCollider==null ? GetComponents<BoxCollider2D>()[0] : playerCollider;

        playerSprite = GetComponent<SpriteRenderer>();
        playerOriginColor = playerSprite==null ? Color.white : playerSprite.color;
        playerAnimator = playerAnimator==null ? GetComponent<Animator>() : playerAnimator;

        adaptationManager = adaptationManager==null ? GetComponentInChildren<AdaptationManager>() : adaptationManager;

        playerSoundPlayer = playerSoundPlayer==null ? GetComponent<SoundPlayer>() : playerSoundPlayer;
    }
    protected override void Start() {
        InitialState();
        InitializeRate();
        ScreenUI.instance.UpdateHPSlider(this);
        ScreenUI.instance.UpdateExpSlider();
        StatusUI.instance.UpdateRateUI();
    }
    protected virtual void InitialState() {
        #region Idle State >>
        idleState.OnActive += (prevState) => {
            playerAnimator.SetBool("Idle", true);

            if(prevState.Compare(floatState)
            || prevState.Compare(JUMP_ATTACK_STATE_TAG)) {
                playerSoundPlayer.PlayClip(playerLandClip);
            }
        };
        idleState.OnInactive += (nextState) => {
            playerAnimator.SetBool("Idle", false);
        };
        #endregion << Idle State

        #region Float State >>
        floatState.OnActive += (prevState) => {
            if(prevState.Compare(floatState))
                playerAnimator.SetTrigger("Double Jump");
            else
                playerAnimator.SetBool("Float", true);
                
            currentJumpCount ++;
        };
        floatState.OnInactive += (nextState) => {
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

            if(prevState.Compare(floatState)
            || prevState.Compare(JUMP_ATTACK_STATE_TAG)) {
                playerSoundPlayer.PlayClip(playerLandClip);
            }
        };
        moveState.OnInactive += (nextState) => {
            playerAnimator.SetBool("Move", false);
            playerAnimator.speed = 1;
        };
        #endregion << Move State

        #region Dodge State >>
        dodgeState.OnActive += (prevState) => {
            if(prevState.Compare(floatState)
            || prevState.Compare(JUMP_ATTACK_STATE_TAG))
                currentJumpCount --;

            dodgeParticle.Play();
            playerSoundPlayer.PlayClip(playerDodgeClip);
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
        RaycastHit2D[] hit = Physics2D.BoxCastAll(playerCollider.bounds.center, playerCollider.bounds.size * new Vector2(1, .98f), 0, new Vector2(moveDirection.x, 0), .02f, GROUNDABLE_LAYER);
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
        playerSoundPlayer.PlayClip(playerJumpClip);
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
        if(playerStateMachine.Compare(ATTACK_STATE_TAG) && !canMove) return;
        
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
        /* || playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG) */) return;
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
        SetExpMaterialAttribute();
        if(abilityPoint>0 && !AbilityChoicesUI.isChoosing) {
            abilityPoint --;
            AbilityManager.instance.OfferChoices();
        }
    }
    protected void ResetDodgeTime() {
        if(cooldownForDodge > 0)
            cooldownForDodge -= Time.deltaTime;
        else if(dodgeCount != maxDodgeCount)
            dodgeCount = maxDodgeCount;
    }
    protected void SetExpMaterialAttribute() {
        expParticleMaterial.SetVector("_PlayerPosition", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
    }
    protected void BasicMove() {
        if(playerStateMachine.Compare(floatState)
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
    public void AnimationEvent_Footstep() {
        try {
            int index = UnityEngine.Random.Range(0, playerFootstepClip.Length);
            playerSoundPlayer.PlayClip(playerFootstepClip[index]);
        } catch(Exception e) {
            Debug.LogWarning(e.StackTrace);
        }
    }
    protected void CheckBottom() {
        if(playerStateMachine.Compare(dodgeState)
        || playerStateMachine.Compare(hitState)
        /* || playerStateMachine.Compare(ATTACK_STATE_TAG) */)
            return;
        Bounds b = playerCollider.bounds;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(b.center.x, b.center.y - b.size.y/2 + .1f), new Vector2(b.size.x, .1f), 0, Vector2.down, .2f, GROUNDABLE_LAYER);

        State next = floatState;
        if(hits.Length <= 0)
            next = floatState;
        else {
            if(playerRigidbody.velocity.y > 0) return;
            foreach(RaycastHit2D hit in hits) {
                string tag = hit.transform.tag;
                if(tag == "Ground"
                || tag == "Platform" && hit.distance >= .05f) {
                    currentJumpCount = 0;
                    next = basicState;
                    break;
                }
            }
        }
        if(next.Compare(basicState) && !playerStateMachine.Compare(ATTACK_STATE_TAG)
        || next.Compare(floatState) && !playerStateMachine.Compare(JUMP_ATTACK_STATE_TAG)) {
            playerStateMachine.ChangeState(next, false);
        }
    }
    public void IncreaseExp(int amount) {
        currentExp += amount;
        if(currentExp >= nextLevelExp) {
            int residual = currentExp - nextLevelExp;
            LevelUp();
            IncreaseExp(0);
        }
        if(increaseExpEvent != null) increaseExpEvent(amount);
        ScreenUI.instance.UpdateExpSlider();
    }
    protected void LevelUp() {
        playerLevel ++;
        abilityPoint ++;
        currentExp -= nextLevelExp;
        nextLevelExp = (int)(nextLevelExp * 1.05f) + 20;
    }
    protected void InitializeRate() {
        try {
            rate = GameManager.instance.gameData.rate;
            rateGauge = GameManager.instance.gameData.rateGauge;
            nextRateGauge = GetNextRateGauge();
        } catch(System.Exception e) {
            Debug.LogWarning(e.StackTrace);
        }
    }
    public void IncreaseRageGauge(int amount) {
        rateGauge += amount;
        if(rateGauge >= nextRateGauge) {
            RateUp();
        }
        GameManager.instance.SetRate(rate, rateGauge);
        StatusUI.instance.UpdateRateUI();
    }
    protected void RateUp() {
        rate ++;
        rateGauge -= nextRateGauge;
        nextRateGauge = GetNextRateGauge();
        adaptationManager.IncreasePoint();
    }
    protected int GetNextRateGauge() {
        int res = 100;
        for(int i=0; i<rate; i++) {
            res = (int)(res * 1.2f) + 10;
        }
        return res;
    }
    protected override float SetHP(float next){
        float nextHp = base.SetHP(next);
        ScreenUI.instance.UpdateHPSlider(this);
        return nextHp;
    }
    private float IncreaseHP(float amount) {
        float nextHp = SetHP(hp + amount);
        return hp;
    }
    public void OnDamage(float damage, float duration=.25f) {
        if(isDead) return;
        IncreaseHP(-damage);
        if(hp <= 0) {
            Die();
        } else {
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(duration));
        }
        UIManager.instance.damageLog.LogDamage((int)damage+"", transform.position, Color.white);
    }
    public void OnDamage(float damage, Color textColor, float duration=.25f) {
        if(isDead) return;
        IncreaseHP(-damage);
        if(hp <= 0) {
            Die();
        } else {
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
            hitCoroutine = StartCoroutine(HitCoroutine(duration));
        }
        UIManager.instance.damageLog.LogDamage((int)damage+"", transform.position, textColor);
    }
    public void OnDamage(float damage, Vector2 force, float duration=.25f) {
        if(isDead) return;
        OnDamage(damage, duration);
        playerSoundPlayer.PlayClip(playerHitClip);
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(force, ForceMode2D.Force);
    }
    public void OnDamage(float damage, Vector2 force, Color textColor, float duration=.25f) {
        if(isDead) return;
        OnDamage(damage, textColor, duration);
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(force, ForceMode2D.Force);
    }
    protected override void Die() {
        base.Die();
        playerDieParticle.SetActive(true);
        playerDieParticle.transform.parent = null;
        playerSoundPlayer.PlayClip(playerDieClip);
        playerSprite.color = new Color(0, 0, 0, 0);
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.layer = 10;
        StartCoroutine(Utility.TimeoutTask(() => {
            GameManager.instance.GameOver();
        }, 2));
    }
    IEnumerator HitCoroutine(float duration) {
        if(duration > 0) {
            playerStateMachine.ChangeState(hitState);
            yield return new WaitForSeconds(duration);
            playerStateMachine.ChangeState(basicState);
        }
    }
    public void ZoomInCamera() {
        normalCamera.SetActive(false);
        zoomCamera.SetActive(true);
    }
    public void ZoomOutCamera() {
        normalCamera.SetActive(true);
        zoomCamera.SetActive(false);
    }
}
