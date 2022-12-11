using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable {
    public abstract void OnDamage(float damage, float duration=0);
    public abstract void OnDamage(float damage, Color textColor, float duration=0);
    public abstract void OnDamage(float damage, Vector2 force, float duration=0);
    public abstract void OnDamage(float damage, Vector2 force, Color textColor, float duration=0);
}