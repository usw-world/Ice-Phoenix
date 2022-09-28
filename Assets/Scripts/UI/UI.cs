using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    [SerializeField] Canvas ownCanvas;

    void Awake() {
        if(ownCanvas == null) {
            ownCanvas = GetComponentInChildren<Canvas>();
            if(ownCanvas == null) {
                Debug.LogError($"UI Object {this.gameObject.name} has not any canvas. Please define 'ownCanvas'.");
            } else {
                Debug.LogWarning($"UI Object {this.gameObject.name}'s 'ownCanvas' was empty. One canvas that {this.gameObject.name} has was defined as ownCanvs.");
            }
        }
    }

    public void OnActive() {
        ownCanvas.enabled = true;
    }
    public void OnInactive() {
        ownCanvas.enabled = false;
    }
}
