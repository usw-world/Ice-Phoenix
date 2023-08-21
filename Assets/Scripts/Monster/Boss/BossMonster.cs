using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectState;

public class BossMonster : Monster {
    const string ATTACK_STATE_TAG = "tag:Attack";

    [SerializeField] GameObject bossRoomActivingObjects;
    [SerializeField] CameraEffect bossRoomCameraEffect;

    StateMachine bossStateMachine;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioClip commonPhaseBGMClip;

    State idleState = new State("Idle");
    State moveState = new State("Move");
    State punchState = new State("Punch", ATTACK_STATE_TAG);
    State summonThunderState = new State("Summon Thunder", ATTACK_STATE_TAG);
    State summonDogsState = new State("Summon Dogs", ATTACK_STATE_TAG);
    State summonMagesState = new State("Summon Mages", ATTACK_STATE_TAG);
    State hitState = new State("Hit");
    State dieState = new State("Die");

    bool isAngry = false;
    bool turningMoveState = false;

    #region Movement
    float moveSpeed = 7f;
    float dirX = -1;
    [SerializeField] AudioClip bossFootStep;
    #endregion Movement

    Transform playerTransform;

    #region Punch
    [SerializeField] AudioClip punchClip;
    [SerializeField] Transform punchEffectPoint;
    [SerializeField] GameObject punchEffect;
    EffectPool punchEffectPool;
    float punchCooldown = 0;
    float punchInterval = 2f;
    #endregion Punch

    #region Summon Dogs
    EffectPool summonEffectPool;
    [SerializeField] GameObject summonEffect;
    [SerializeField] GameObject[] shadowDogs;
    float summonDogsCooldown = 5f;
    float summonDogsInterval = 15f;
    #endregion Summon Dogs

    #region Summon Mages
    [SerializeField] GameObject[] shadowMages;
    float summonShadowMagesCooldown = 15;
    float summonShadowMagesInterval = 30f;
    #endregion Summon Mages

    #region Thunder Calling
    [SerializeField] GameObject thunder;
    EffectPool thunderPool;
    float thunderCooldown = 2f;
    float thunderInterval = 8f;
    #endregion Thunder Calling
    
    [SerializeField] AudioClip shoutClip;

    protected override void Awake() {
        base.Awake();
        bossStateMachine = GetComponent<StateMachine>();
        bossStateMachine.SetIntialState(idleState);
    }
    protected override void Start() {
        base.Start();
        playerTransform = Player.playerInstance.transform;
        InitializeStates();
        if(playerTransform != null) {
            Decide();
        };
        punchEffectPool = new EffectPool("Boss Punch Effect", punchEffect, 5, 2, null);
        summonEffectPool = new EffectPool("Summon Effect", summonEffect);
        thunderPool = new EffectPool("Thunder", thunder, 20, 10);
    }
    private void InitializeStates() {
        idleState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Idle", true);
        };
        idleState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Idle", false);
        };
        moveState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Move", true);
        };
        moveState.OnStay = () => {
            LookAtX(dirX);
            Decide();
        };
        moveState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Move", false);
            Stop();
        };
        
        punchState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Punch", true);
        };
        punchState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Punch", false);
        };
        
        summonThunderState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Thunder Calling", true);
        };
        summonThunderState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Thunder Calling", false);
        };


        summonDogsState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Summon Dogs", true);
        };
        summonDogsState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Summon Dogs", false);
        };
        
        summonMagesState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Summon Soldiers", true);
        };
        summonMagesState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Summon Soldiers", false);
        };

        dieState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Die", true);
        };
        dieState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Die", false);
        };
    }
    private void Decide() {
        if(bossStateMachine.currentState.Compare(idleState)) {
            bossStateMachine.ChangeState(moveState);
            turningMoveState = true;
            return;
        }

        if(turningMoveState) return;

        if(isAngry
        && thunderCooldown <= 0) {
           thunderCooldown = thunderInterval;
           bossStateMachine.ChangeState(summonThunderState);
        }

        if(summonShadowMagesCooldown <= 0) {
            summonShadowMagesCooldown = summonShadowMagesInterval;
            bossStateMachine.ChangeState(summonMagesState);
            return;
        }

        if(summonDogsCooldown <= 0) {
            summonDogsCooldown = summonDogsInterval;
            bossStateMachine.ChangeState(summonDogsState);
            return;
        }

        try {
            if(Mathf.Abs(playerTransform.position.x - transform.position.x)< 12
            && punchCooldown <= 0) {
                bossStateMachine.ChangeState(punchState);
                return;
            }
        } catch(System.Exception e) {
            print(e.StackTrace);
            return;
        }
    }
    protected override void Update() {
        base.Update();
        try {
            dirX = playerTransform.position.x-transform.position.x<0 ? -1 : 1;
        } catch {
            dirX = -1;
        }

        if(punchCooldown > 0)
            punchCooldown -= Time.deltaTime;
        if(summonDogsCooldown > 0)
            summonDogsCooldown -= Time.deltaTime;
        if(summonShadowMagesCooldown > 0)
            summonShadowMagesCooldown -= Time.deltaTime;
        if(thunderCooldown > 0)
            thunderCooldown -= Time.deltaTime;
    }

    protected override void LookAtX(float x) {
        if(x > 0) {
            transform.localScale = new Vector3(6, 6, 6);
            if(rotatelessChildren != null) rotatelessChildren.localScale = new Vector3(1, 1, 1);
        } else if (x < 0) {
            transform.localScale = new Vector3(-6, 6, 6);
            if(rotatelessChildren != null) rotatelessChildren.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    public override void OnDamage(float damage, float duration=.25f) {
        base.OnDamage(damage, duration);
        if(hp < maxHp/2) {
            isAngry = true;
            monsterAnimator.speed = 1.25f;
            SpriteRenderer renderer;
            if(TryGetComponent<SpriteRenderer>(out renderer)) {
                renderer.color = new Color(.5f, 0, .15f);
            }
        }
        if(hp <= 0) {
            Die();
            bossStateMachine.ChangeState(dieState);
            OnDie();
        }
    }
    private void OnDie() {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = commonPhaseBGMClip;
        bgmAudioSource.Play();

        GameManager.instance.IncreaseClearCount();
        foreach(GameObject dog in shadowDogs) {
            if(!dog.GetComponent<LivingEntity>().isDead && dog.activeInHierarchy)
                dog.GetComponent<IDamageable>().OnDamage(dog.GetComponent<LivingEntity>().maxHp);
        }
        foreach(GameObject mage in shadowMages) {
            if(!mage.GetComponent<LivingEntity>().isDead && mage.activeInHierarchy)
                mage.GetComponent<IDamageable>().OnDamage(mage.GetComponent<LivingEntity>().maxHp);
        }

        Player.playerInstance.IncreaseRateGauge(2200);
        Player.playerInstance.isInvincible = true;
    }

    public void Move() {
        monsterRigidbody.velocity = new Vector2(dirX * moveSpeed, 0);
    }
    public void Stop() {
        monsterRigidbody.velocity = Vector2.zero;
    }

    public void AnimationEvent_PunchDamage() {
        punchCooldown = punchInterval;
        GameObject effect = punchEffectPool.OutPool(punchEffectPoint.position, null);
        effect.transform.localScale = transform.localScale.x>0 ? new Vector3(-3, 3, 3) : new Vector3(3, 3, 3);
        effect.GetComponent<BossPunchEffect>().endEvent = () => {
            punchEffectPool.InPool(effect);
        };
        monsterRigidbody.AddForce(Vector3.Scale(transform.localScale, -Vector3.right) * 300);
        monsterSoundPlayer.PlayClip(punchClip);
    }
    public void AnimationEvent_Footstep() {
        monsterSoundPlayer.PlayClip(bossFootStep);
        bossRoomCameraEffect.Shake(3f, .1f);
        turningMoveState = false;
    }
    public void AnimationEvent_SetMove() {
        turningMoveState = true;
        bossStateMachine.ChangeState(moveState);
    }
    public void AnimationEvent_CameraShake(float second) {
        bossRoomCameraEffect.Shake(3f, second);
    }
    public void AnimationEvent_Shout() {
        monsterSoundPlayer.PlayClip(shoutClip);
    }
    public void AnimationEvent_SummonDogs() {
        StartCoroutine(IntervalCoroutine((int index) => {

            GameObject dog = shadowDogs[index];
            dog.SetActive(true);
            dog.GetComponent<ShadowDog>().Revive();
            
            GameObject s_effect = summonEffectPool.OutPool(dog.transform.position + new Vector3(0, -.5f, 0));
            
            StartCoroutine(Utility.TimeoutTask(() => {
                summonEffectPool.InPool(s_effect);
            }, 4));
        }, .2f, shadowDogs.Length));
    }
    public void AnimationEvent_SummonMages() {
        foreach(GameObject mage in shadowMages) {
            mage.SetActive(true);
            mage.GetComponent<ShadowMage>().Revive();
            GameObject s_effect = summonEffectPool.OutPool(mage.transform.position/*  + new Vector3(0, -.5f, 0) */);
            StartCoroutine(Utility.TimeoutTask(() => {
                summonEffectPool.InPool(s_effect);
            }, 4));
        }
    }
    public void AnimationEvent_Thunder() {
        StartCoroutine(IntervalCoroutine((int i) => {
            Vector2 pos = new Vector2(i*5+45, -.2f);
            GameObject effect = thunderPool.OutPool(pos, null);
            effect.GetComponent<BossThunderbolt>().endEvent = () => {thunderPool.InPool(effect);};
        }, .2f, 9));
    }
    public IEnumerator IntervalCoroutine(System.Action<int> func, float interval, int count) {
        for(int i=0; i<count; i++) {
            func(i);
            yield return new WaitForSeconds(interval);
        }
    }
    public void OnClear() {
        bossRoomActivingObjects.SetActive(false);
    }
}