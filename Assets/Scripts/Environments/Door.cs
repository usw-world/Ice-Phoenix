using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField] GameManager.SceneList targetScene;
    bool hasEnter = false;

    [SerializeField] private int increasingRateGauge;

    void Start() {
        // if(GameManager.instance.gameData != null) 
        //     increasingRateGauge *= (int)(GameManager.instance.gameData.clearCount * 1.2f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(hasEnter) return;
        if(other.tag == "Player") {
            Player.playerInstance.IncreaseRateGauge(increasingRateGauge);
            
            Player player;
            hasEnter = true;
            if(other.TryGetComponent<Player>(out player)) {
                UIManager.instance.FadeOut(() => {
                    GameManager.instance.ChangeScene(targetScene);
                });
            }
        }
    }
}