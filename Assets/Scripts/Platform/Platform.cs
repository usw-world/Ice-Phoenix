using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    PlatformEffector2D platformEffector;
    PlatformEffector2D GetPlatformEffector {
        get {
            if(platformEffector == null)
                return GetComponent<PlatformEffector2D>();
            else
                return platformEffector;
        }
    }
    bool isDeactive = false;
    int? originMask = null;
    int GetOriginMask {
        get {
            if(!originMask.HasValue)
                try {
                    return platformEffector.colliderMask;
                } catch(NullReferenceException e) {
                    Debug.LogError(e.StackTrace);
                    return -1;
                }
            else
                return originMask.Value;
        }
    }

    void Awake() {
        platformEffector = GetComponent<PlatformEffector2D>();
        if(platformEffector != null) {
            originMask =  platformEffector.colliderMask;
        }
    }

    public void DisablePlatform() {
        if(isDeactive) return;

        if(platformEffector == null) {
            platformEffector = GetComponent<PlatformEffector2D>();
        }
        if(platformEffector != null) {
            originMask = originMask ?? platformEffector.colliderMask;
            platformEffector.colliderMask = ~8 & platformEffector.colliderMask;
            isDeactive = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(1<<other.gameObject.layer == Player.DEFAULT_PLAYER_LAYERMASK) {
            isDeactive = false;
            platformEffector.colliderMask = originMask.Value;
        }
    }
}
