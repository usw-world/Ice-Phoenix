using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attach : MonoBehaviour {
    public abstract string attchType { get; }
    public LivingEntity attachedTarget;
    public System.Action detachEvent;
    public void Update() {
        OnStayAttach();
    }
    public abstract void OnAttach();
    public abstract void OnStayAttach();
    public abstract void OnDetach();

    public bool Equals(Attach other) {
        // print(other.attchType);
        return attchType == other.attchType;
    }
    public override bool Equals(object other) {
        return this.GetType().Equals(other.GetType());
        // return base.Equals(other);
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        hash.Add(attchType);
        return hash.ToHashCode();
    }
}