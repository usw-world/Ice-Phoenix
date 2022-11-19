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
    public void InitializeParticle(string particleName, GameObject particle, int amount=10, int resizeAmount=5) {
        ParticlePool pool = new ParticlePool(particleName, particle, amount, resizeAmount);
        for(int i=0; i<amount; i++) {
            pool.InPool(Instantiate(particle, transform), transform);
        }
        particleMap.Add(particleName, pool);
    }
    public GameObject Call(string particleName, Vector2 point, Transform parent=null) {
        ParticlePool pool = particleMap[particleName];
        if(pool.Count <= 0) {
            RestorePool(pool);
        }
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
    public void RestorePool(ParticlePool pool) {
        for(int i=0; i<pool.resizeAmount; i++) {
            pool.InPool(Instantiate(pool.poolingObject, transform), transform);
        }
    }
    public void Release(GameObject target) {
        PoolingData data = target.GetComponent<PoolingData>();
        if(data != null) {
            particleMap[data.poolName].InPool(target, transform);
        } else {
            Debug.LogWarning("Object that incomming into Pool is not 'Pooling Object'.");
        }
    }
    public void Release(GameObject target, float second) {
        StartCoroutine(Utility.CoroutineTask(() => {
            Release(target);
        }, second));
    }
}