using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
    public float damage = 5f;
    public float force = 200f;
    public float explosionRadius = 2f;

    public System.Action explosionEvent;
    
    void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == Monster.DEFALUT_MONSTER_LAYER) {
            Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, explosionRadius, Monster.DEFALUT_MONSTER_LAYER);
            foreach(Collider2D inner in inners) {
                float dirX = inner.transform.position.x-transform.position.x>0 ? 1 : -1;
                inner.GetComponent<IDamageable>().OnDamage(damage, new Vector2(dirX * force, force), new Color(1, .5f, 0), .25f);
                gameObject.SetActive(false);
            }
            if(explosionEvent != null)
                explosionEvent();
        }
    }
}
