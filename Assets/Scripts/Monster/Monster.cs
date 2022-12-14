using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : LivingEntity, IDamageable {
    static public int DEFALUT_MONSTER_LAYER_NUMBER = 7;
    static public int DEFALUT_MONSTER_LAYER = 128;
    #region Sound
    [Header("Monster Sound Clips")]
    [SerializeField] protected SoundPlayer monsterSoundPlayer;
    [SerializeField] protected AudioClip[] monsterAttackClip;
    [SerializeField] protected AudioClip monsterDeathClip;
    [SerializeField] protected AudioClip[] monsterHitClip;
    #endregion Sound

    [Header("Monster Details")]
    [SerializeField] protected SideUI monsterSideUI;
    
    [SerializeField] protected Animator monsterAnimator;
    [SerializeField] protected Rigidbody2D monsterRigidbody;


    public void GameSet() {
        Destroy(this);
    }
    protected override void Awake() {
        base.Awake();

        try {
            SetMaxHP(maxHp + GameManager.instance.gameData.clearCount * .15f, true);
        } catch(System.Exception e) {
            print(e.StackTrace);
        }

        monsterAnimator = GetComponent<Animator>();
        if(monsterAnimator == null)
            Debug.LogWarning($"Monster hasn't any 'Animator'.");

        if (monsterRigidbody==null && !TryGetComponent<Rigidbody2D>(out monsterRigidbody))
            Debug.LogWarning("Monster hasn't any 'Rigidbody2D'.");
    }
    protected override void OnEnable() {
        base.OnEnable();
        if(monsterSideUI != null)
            monsterSideUI.UpdateHPSlider(this);
        else
            Debug.LogWarning("Monster hasn't any 'Side UI'.");
    }
    protected virtual void Update() {}

    private float IncreasHP(float amount) {
        float nextHp = SetHP(hp + amount);
        return hp;
    }
    protected override float SetHP(float next) {
        base.SetHP(next);
        if(monsterSideUI != null)
            monsterSideUI.UpdateHPSlider(this);
        return hp;
    }
    public virtual void OnDamage(float damage, float duration=.25f) {
        if(monsterHitClip.Length > 0) {
            int soundIndex = Random.Range(0, monsterHitClip.Length);
            monsterSoundPlayer.PlayClip(monsterHitClip[soundIndex]);
        }
        IncreasHP(-damage);
        if(monsterSideUI != null)
            monsterSideUI.UpdateHPSlider(this);
    }
    public virtual void OnDamage(float damage, Color color, float duration=.25f) {
        OnDamage(damage, duration);
        UIManager.instance.damageLog.LogDamage((int)damage+"", transform.position, color);
    }
    public virtual void OnDamage(float damage, Vector2 force, float duration=.25f) {
        OnDamage(damage, duration);
    }
    public virtual void OnDamage(float damage, Vector2 force, Color color, float duration=.25f) {
        OnDamage(damage, force, duration);
        UIManager.instance.damageLog.LogDamage((int)damage+"", transform.position, color);
    }

    protected override void Die() {
        base.Die();
        if(Player.playerInstance.onDefeatMonster != null)
            Player.playerInstance.onDefeatMonster(this);
        if(monsterDeathClip != null) {
            monsterSoundPlayer.PlayClip(monsterDeathClip);
        }
        gameObject.layer = 10;
        GiveExperience giveExperience;
        if(TryGetComponent<GiveExperience>(out giveExperience)) {
            giveExperience.ReleaseExp();
        }
    }
}