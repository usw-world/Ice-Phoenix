using UnityEngine;
using AbilitySystem;

public class Ability_Bleeding : Ability {
    public override int maxLevel => 3;
    
    float[] damagesPerSecond = { 8, 10, 12 };
    float[] maxAttachCount = { 3, 5, 7 };
    float duration = 5f;

    [SerializeField] private AttachPooler bleedingAttachPool;

    [SerializeField] private Attach_Bleeding att_bleeding;

    public void Start() {
        bleedingAttachPool = new AttachPooler("Bleeding", att_bleeding, 10, 5, null);
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        Player.playerInstance.basicAttackDamageEvent += AttachBleedingToTarget;
    }
    public override void OnReleaseAbility() {
        Player.playerInstance.basicAttackDamageEvent -= AttachBleedingToTarget;
    }
    public void AttachBleedingToTarget(Transform target) {
        LivingEntity le = target.GetComponent<LivingEntity>();
        var bleeding = bleedingAttachPool.OutPool(le.transform.position, le.transform) as Attach_Bleeding;
        /*  */
        bleeding.duration = 4f;
        bleeding.damagePerSecond = 10f;
        /*  */
        le.GetAttach(bleeding);
    }
}