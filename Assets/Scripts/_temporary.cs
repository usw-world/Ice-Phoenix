using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _temporary : MonoBehaviour {
    [SerializeField] ParticleManager pm;
    [SerializeField] GameObject t_particle;

    void Start() {
        pm.InitializeParticle("Temp", t_particle);
    }
    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            GameObject gobj = pm.Call("Temp", GameObject.FindWithTag("Player").transform.position, 2, GameObject.FindWithTag("Player").transform);
        }
    }
}
