using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attach : MonoBehaviourIF {
    protected LivingEntity attachedEntity;
    public void Update() {
        OnStayAttach();
    }
    public virtual void AttachTo(LivingEntity target) {
        attachedEntity = target;
        OnAttach();
    }
    public virtual void Detach() {
        OnDetach();
        attachedEntity = null;
    }
    protected abstract void OnAttach();
    protected abstract void OnStayAttach();
    protected abstract void OnDetach();
}