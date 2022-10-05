using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LivingEntity : Entity {
    bool isDead = false;
    public virtual void Die() {
        isDead = true;
    }
}