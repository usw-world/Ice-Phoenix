using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonoBehaviourIF : MonoBehaviour {
    [Header("MonoBehavior IF")]
    [SerializeField] Transform rotatelessChildren;
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