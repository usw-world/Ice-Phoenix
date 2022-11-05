using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {
    // [SerializeField] GameObject damageEffect; //2022-11-04추가
    Dictionary<string, GameObject> particleMap = new Dictionary<string, GameObject>();

    public void Start() {}
    public void InitializeParticle(string particleName, GameObject particle) {
        particleMap.Add(particleName, particle);
    }
    public GameObject Generate(string target, Vector2 point) {
        if(particleMap[target] != null)          //2022-11-04추가
            return Instantiate(particleMap[target], new Vector2(point.x, point.y), Quaternion.identity);//2022-11-04추가
        return null;
    }
    public GameObject Generate(string target, Vector2 point, Quaternion rotation) {
        if(particleMap[target] != null)
            return Instantiate(particleMap[target], new Vector2(point.x, point.y), rotation);
        return null;
    }
    public void Release(GameObject target) {
        Destroy(target);
    }
    public void Release(GameObject target, float second) {
        StartCoroutine(Utility.CoroutineTask(() => {
            Release(target);
        }, second));
    }
}