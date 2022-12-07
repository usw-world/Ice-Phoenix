using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attach : MonoBehaviour {
    public LivingEntity attachedEntity;
    public void Update() {
        OnStayAttach();
    }
    public abstract void OnAttach();
    public abstract void OnStayAttach();
    public abstract void OnDetach();
}