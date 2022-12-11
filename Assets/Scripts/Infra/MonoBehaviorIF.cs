using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonoBehaviourIF : MonoBehaviour {
    [System.Serializable]
    public struct Area {
        public Vector2 center;
        public float radius;
    }
    [System.Serializable]
    public struct Range {
        public Vector2 center;
        public Vector2 bounds;
        public Range(Vector2 center, Vector2 bounds) {
            this.center = center;
            this.bounds = bounds;
        }
    }
    [Header("MonoBehavior IF")]
    [SerializeField] public Transform rotatelessChildren;
    protected void LookAtX(float x) {
        if(x > 0) {
            transform.localScale = new Vector3(1, 1, 1);
            if(rotatelessChildren != null) rotatelessChildren.localScale = new Vector3(1, 1, 1);
        } else if (x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
            if(rotatelessChildren != null) rotatelessChildren.localScale = new Vector3(-1, 1, 1);
        }
    }
}