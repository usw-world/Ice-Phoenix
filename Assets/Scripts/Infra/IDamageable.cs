using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable {
    public abstract void OnDamage(float damage, float duration);
    public abstract void OnDamage(float damage, Vector2 force, float duration);
}