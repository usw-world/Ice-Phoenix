using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class AbilityFury : Ability {
    Player player;
    [SerializeField] GameObject playerObject;
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
    float[] increasingPercents = {15, 40, 65};
    // 공격력 상승량 15% / 40% / 65%

    public override void OnGetAbility() {
        print(player == null);
        player.damageCoefs += ReturnCofficient;
    }
    public override void OnReleaseAbility() {
        player.damageCoefs -= ReturnCofficient;
    }
    public float ReturnCofficient() {
        /*
            - 체력 20%일 때 최대 상승
            - 예시) 특성 레벨 2일 때 체력이 60%(20% + (80%*0.5))라면,
            -       40%(특성 레벨 2일 때 최대치)의 1/2인 20%p 상승
            - (x - (x * ((y - 0.2) / 0.8))) << 현재 상승치
            - x = 최대 상승치, y = 현재 체력 비율
        */
        float offset = player.hpRatio * .8f;
        return Mathf.Lerp(increasingPercents[level], 0, offset+.2f) * 0.01f;
    }
}