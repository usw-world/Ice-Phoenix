using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class __test_dummy_pooler : MonoBehaviour {
    [SerializeField] GameObject __dummy_monster;
    [SerializeField] public int __count = 0;

    Pool<GameObject> __dummy_pool;
    void Start() {
        __dummy_pool = new MonsterPooler("Dummy", __dummy_monster);
    }
    public int __c {
        set {
            __count = value;
        }
    }
    public void __spawn() {
        StartCoroutine(__spawn_coroutine());
    }
    private IEnumerator __spawn_coroutine() {
        for(int i=0; i<__count; i++) {
            GameObject gobj = __dummy_pool.OutPool(transform.position, null);
            yield return new WaitForSeconds(.5f);
        }
    }
}
