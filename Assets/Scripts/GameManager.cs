using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance { get; private set; }

    public void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public void StopGame() {
        Time.timeScale = 0;
    }
    public void StartGame() {
        Time.timeScale = 1;
    }
}
