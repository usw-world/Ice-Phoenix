using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleeding : Attach {
    [SerializeField] GameObject bleedingEffect;
    private EffectPool bleedingEffectPool;

    float lifetime = 0f;
    float duration = 0f;
    float damagePerSecond = 0;
    float interval = 1f;
    
    GameObject showingEffect = null;

    protected void Awake() {
        bleedingEffectPool = new EffectPool("Bleeding Effect", bleedingEffect, 10, 5);
    }
    public override void AttachTo(LivingEntity target) {
        base.AttachTo(target);
    }
    public override void Detach() {
        base.Detach();
    }
    protected override void OnAttach() {
        Transform target = attachedEntity.transform;
        showingEffect = bleedingEffectPool.OutPool(target.position, target);
        StartCoroutine(DurationCoroutine());
    }
    protected override void OnDetach() {
        bleedingEffectPool.InPool(showingEffect);
    }
    protected override void OnStayAttach() {}
    private IEnumerator DurationCoroutine() {
        while(lifetime < duration) {
            yield return new WaitForSeconds(interval);
            IDamageable idmg;
            if(attachedEntity.TryGetComponent<IDamageable>(out idmg)) {
                idmg.OnDamage(damagePerSecond, 0);
            }
            lifetime += interval;
        }
        OnEndDuration();
    }
    private void OnEndDuration() {
        Detach();
    }
}