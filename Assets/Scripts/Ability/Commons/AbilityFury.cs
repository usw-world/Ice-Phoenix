using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class AbilityFury : Ability {
    Player player;
    GameObject playerObject;
    protected void Awake() {
        if(playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject == null)
            playerObject = GameObject.Find("Player");
        if(playerObject != null)
            player = playerObject.GetComponent<Player>();
        if(player == null)
            Debug.Log("player('Player' type) variable is null.");
    }
    protected override int maxLevel {
        get { return 3; }
    } // 최대 3레벨
    float[] increasingPercents = {.15f, .40f, .65f};
    // 공격력 상승량 15% / 40% / 65%

    public override void OnGetAbility() {
        player.damageCoefs += ReturnCofficient;
    }
    public override void OnReleaseAbility() {
        player.damageCoefs -= ReturnCofficient;
    }
    public float ReturnCofficient() {
        return Mathf.Lerp(0, increasingPercents[level], (0.8f - (player.hpRatio - 0.2f)) / 0.8f);
    }
}