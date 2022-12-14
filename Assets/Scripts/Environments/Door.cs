using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField] GameManager.SceneList targetScene;
    bool hasEnter = false;

    void OnTriggerEnter2D(Collider2D other) {
        if(hasEnter) return;
        hasEnter = true;
        Player player;
        if(other.tag == "Player") {
            if(other.TryGetComponent<Player>(out player)) {
                UIManager.instance.FadeOut(() => {
                    GameManager.instance.ChangeScene(targetScene);
                });
            }
        }
    }
}