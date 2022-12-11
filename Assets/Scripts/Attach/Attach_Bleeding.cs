using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach_Bleeding : Attach {
    private EffectPool bleedingEffectPool;

    public float lifetime = 0f;
    public float duration = 0f;
    public float damagePerSecond = 0;
    public float interval = 1f;
    public int maxAttachedCount = 1;

    public int attachedCount = 0;

    Coroutine damageCoroutine;

    public override string attchType => "Bleeding";

    public override void OnAttach() {
        transform.localPosition = Vector3.zero;
        damageCoroutine = StartCoroutine(DurationCoroutine());
    }
    public override void OnDetach() {
        if(damageCoroutine != null) StopCoroutine(damageCoroutine);
        if(detachEvent != null) detachEvent();
    }
    public override void OnStayAttach() {}
    private IEnumerator DurationCoroutine() {
        while(lifetime < duration) {
            yield return new WaitForSeconds(interval);
            IDamageable idmg;
            if(attachedTarget.TryGetComponent<IDamageable>(out idmg)) {
                idmg.OnDamage(damagePerSecond * attachedCount, new Color(.8f, 0, 0), 0);

            }
            lifetime += interval;
        }
        OnEndDuration();
    }
    private void OnEndDuration() {
        if(attachedTarget != null) 
            attachedTarget.ReleaseAttach(this);
    }
}