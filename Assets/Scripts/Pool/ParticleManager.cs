using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class ParticleManager : MonoBehaviour {
    static public ParticleManager instance { get; private set; }
    Dictionary<string, ParticlePool> particleMap = new Dictionary<string, ParticlePool>();

    public void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public void Start() {}
    public bool ContainParticle(string particleName) {
        return particleMap.ContainsKey(particleName);
    }
    public void InitializeParticle(string particleName, GameObject particle, int amount=10, int resizeAmount=5) {
        ParticlePool pool = new ParticlePool(particleName, particle, amount, resizeAmount, transform);
        particleMap.Add(particleName, pool);
    }
    public GameObject Call(string particleName, Vector2 point, Transform parent=null) {
        ParticlePool pool = particleMap[particleName];
        GameObject particle = particleMap[particleName].OutPool(point, parent);
        particle.AddComponent<PoolingData>();
        particle.GetComponent<PoolingData>().poolName = particleName;
        return particle;
    }
    public GameObject Call(string particleName, Vector2 point, int second, Transform parent=null) {
        GameObject particle = Call(particleName, point, parent);
        Release(particle, second);
        return particle;
    }
    public void Release(GameObject target) {
        PoolingData data = target.GetComponent<PoolingData>();
        if(data != null) {
            particleMap[data.poolName].InPool(target);
        } else {
            Debug.LogWarning("Object that incomming into Pool is not 'Pooling Object'.");
        }
    }
    public void Release(GameObject target, float second) {
        StartCoroutine(Utility.TimeoutTask(() => {
            Release(target);
        }, second));
    }
}