using UnityEngine;
using AbilitySystem;

public class Ability_Bleeding : Ability {
    public override int maxLevel => 3;
    
    float[] damagesPerSecond = { 2, 3, 4 };
    int[] maxAttachCount = { 3, 5, 7 };
    float duration = 5f;

    [SerializeField] private AttachPooler bleedingAttachPool;

    [SerializeField] private Attach_Bleeding att_bleeding;

    public void Start() {
        bleedingAttachPool = new AttachPooler("Bleeding", att_bleeding, 10, 5, transform);
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.basicAttackDamageEvent += AttachBleedingToTarget;
        Player.playerInstance.jumpAttackDamageEvent += AttachBleedingToTarget;
    }
    public override void OnReleaseAbility() {
        Player.playerInstance.basicAttackDamageEvent -= AttachBleedingToTarget;
        Player.playerInstance.jumpAttackDamageEvent -= AttachBleedingToTarget;
    }
    public void AttachBleedingToTarget(Transform target) {
        LivingEntity le = target.GetComponent<LivingEntity>();
        if(le.isDead) return;
        
        Attach att;
        if(le.havingAttach.Find(att_bleeding, out att)) {
            Attach_Bleeding att_b = att as Attach_Bleeding;
            att_b.lifetime = 0;
            att_b.damagePerSecond = damagesPerSecond[level];
            if(att_b.attachedCount < maxAttachCount[level]) {
                att_b.attachedCount ++;
            }
        } else {
            var bleeding = bleedingAttachPool.OutPool(le.transform.position, le.transform) as Attach_Bleeding;
            bleeding.lifetime = 0;
            bleeding.duration = duration;
            bleeding.damagePerSecond = damagesPerSecond[level] * Player.playerInstance.abilityCoef;
            bleeding.maxAttachedCount = maxAttachCount[level];
            bleeding.attachedCount = 1;
            bleeding.detachEvent = () => {
                bleedingAttachPool.InPool(bleeding);
            };
            le.GetAttach(bleeding);
        }
    }
}