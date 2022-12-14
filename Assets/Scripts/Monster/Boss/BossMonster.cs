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

    float moveSpeed = 7f;
    float dirX = -1;

    Transform playerTransform;

    [SerializeField] Transform punchEffectPoint;
    [SerializeField] GameObject punchEffect;
    EffectPool punchEffectPool;
    float punchingCooldown = 0;
    float punchInterval = 2f;
    
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
            bossStateMachine.ChangeState(moveState);
        };
        punchEffectPool = new EffectPool("Boss Punch Effect", punchEffect, 5, 2, null);
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
            if(Mathf.Abs(playerTransform.position.x - transform.position.x)< 12
            && punchingCooldown <= 0) {
                bossStateMachine.ChangeState(punchState);
            }
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
        
        summonThunderState.OnActive = (State prevState) => {};
        summonThunderState.OnInactive = (State nextState) => {};
        
        summonDogsState.OnActive = (State prevState) => {};
        summonDogsState.OnInactive = (State nextState) => {};
        
        summonMagesState.OnActive = (State prevState) => {};
        summonMagesState.OnInactive = (State nextState) => {};
        
        hitState.OnActive = (State prevState) => {};
        hitState.OnInactive = (State nextState) => {};

        dieState.OnActive = (State prevState) => {
            monsterAnimator.SetBool("Die", true);
        };
        dieState.OnInactive = (State nextState) => {
            monsterAnimator.SetBool("Die", false);
        };
    }
    protected override void Update() {
        base.Update();
        dirX = playerTransform.position.x-transform.position.x<0 ? -1 : 1;

        if(punchingCooldown > 0) {
            punchingCooldown -= Time.deltaTime;
        }
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
            SpriteRenderer renderer;
            if(TryGetComponent<SpriteRenderer>(out renderer)) {
                renderer.color = new Color(.5f, 0, .15f);
            }
        }
        if(hp <= 0) {
            Die();
            bossStateMachine.ChangeState(dieState);

            bgmAudioSource.Stop();
            bgmAudioSource.clip = commonPhaseBGMClip;
            bgmAudioSource.Play();
        }
    }

    public void Move() {
        monsterRigidbody.velocity = new Vector2(dirX * moveSpeed, 0);
    }
    public void Stop() {
        monsterRigidbody.velocity = Vector2.zero;
    }

    public void AnimationEvent_PunchDamage() {
        punchingCooldown = punchInterval;
        GameObject effect = punchEffectPool.OutPool(punchEffectPoint.position, null);
        effect.transform.localScale = transform.localScale.x>0 ? new Vector3(-3, 3, 3) : new Vector3(3, 3, 3);
        effect.GetComponent<BossPunchEffect>().endEvent = () => {
            punchEffectPool.InPool(effect);
        };
        monsterRigidbody.AddForce(Vector3.Scale(transform.localScale, -Vector3.right) * 300);
    }
    public void AnimationEvent_PunchEnd() {
        bossStateMachine.ChangeState(moveState);
    }
    public void AnimationEvent_CameraShake(float second) {
        bossRoomCameraEffect.Shake(.2f, second);
    }
    public void AnimationEvent_Shout() {
        monsterSoundPlayer.PlayClip(shoutClip);
    }
    public void OnClear() {
        bossRoomActivingObjects.SetActive(false);
    }
}