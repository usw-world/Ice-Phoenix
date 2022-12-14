using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class Ability_FireRing : Ability {
    [SerializeField] GameObject fireRingPrefab;
    GameObject fireRing;
    float[] damage = { 14, 18, 22, 26, 30 };
    public float Damage {
        get { return damage[level]; }
    }
    float[] respawnTime = { 7, 6, 5, 4, 3 };
    public float RespawnTime {
        get { return respawnTime[level]; }
    }
    public override int maxLevel {
        get { return 5; }
    }
    public override void OnGetAbility() {
        base.OnGetAbility();
        fireRing = Instantiate(fireRingPrefab, Player.playerInstance.rotatelessChildren);
        fireRing.GetComponent<FireRing>().owner = this;
        fireRing.SetActive(true);
        fireRing.transform.localPosition = Vector3.zero;
    }
    public override void OnReleaseAbility() {
        if(fireRing != null) {
            fireRing.SetActive(false);
            Destroy(fireRing);
        }
    }
}