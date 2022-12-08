using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach_Bleeding : Attach {
    private EffectPool bleedingEffectPool;

    private float lifetime = 0f;
    public float duration = 0f;
    public float damagePerSecond = 0;
    public float interval = 1f;

    int attachedCount = 0;

    // protected void Awake() {}
    public override void OnAttach() {
        Transform target = attachedEntity.transform;
        transform.localPosition = Vector3.zero;
        StartCoroutine(DurationCoroutine());
    }
    public override void OnDetach() {
        transform.parent = null;
    }
    public override void OnStayAttach() {}
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
        attachedEntity.ReleaseAttach(this);
    }
}