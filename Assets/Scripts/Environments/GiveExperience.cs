using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveExperience : MonoBehaviour {
    [SerializeField] private int expAmount = 5;
    [SerializeField] private GameObject expObject;

    public void Start() {
        if(expObject == null)
            Debug.LogError("Game object might throw 'NullReferenceException'." + expObject);
    }
    
    public void ReleaseExp() {
        
    }
}