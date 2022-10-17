using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    PlatformEffector2D platformEffector;
    public bool isInactive { get; private set; } = false;
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
        Collider2D[] colliders = GetComponents<Collider2D>();
        if(colliders.Length < 2) {
            Debug.LogWarning($"If a GameObject that has {this.name} Script hasn't collider 2 or more, Maybe {this.name} Script will not working well.");
        } else {
            bool hasCollider = false;
            bool hasTrigger = false;
            foreach(Collider2D collider in colliders) {
                if(collider.isTrigger == true) 
                    hasTrigger = true;
                else
                    hasCollider = true;
            }
            if(!hasCollider || !hasTrigger) {
                Debug.LogWarning($"{this.name} Script need 2 collider each 'Collider' and 'Trigger' to work.");
            }
        }
    }

    public void DisablePlatform() {
        if(isInactive) return;

        if(platformEffector == null) {
            platformEffector = GetComponent<PlatformEffector2D>();
        }
        if(platformEffector != null) {
            originMask = originMask ?? platformEffector.colliderMask;
            platformEffector.colliderMask = ~8 & platformEffector.colliderMask;
            isInactive = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(1<<other.gameObject.layer == Player.DEFAULT_PLAYER_LAYERMASK) {
            isInactive = false;
            platformEffector.colliderMask = originMask.Value;
        }
    }
}
