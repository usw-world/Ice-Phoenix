using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow_Magic : MonoBehaviourIF {
    public System.Action endEvent;

    public Transform target;

    float damage = 20;
    float moveSpeed = 3.5f;

    private void Start() {
        StartCoroutine("RemoveMagicOverTime");
    }
    private void Update() {
        MoveToTarget();
    }

    private void MoveToTarget() {
        try {
            transform.Translate((target.position - transform.position).normalized * Time.deltaTime * moveSpeed * (Mathf.Sin(Time.time) + 1));
        } catch {
            Destroy(gameObject);
        }
    }

    private IEnumerator RemoveMagicOverTime()
    {
        float removeTime = 7f;
        yield return new WaitForSeconds(removeTime);
        endEvent();
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            Collider2D inner = player.GetComponent<Collider2D>();
            IDamageable target = inner.GetComponent<IDamageable>();
            if (target != null)
            {
                Vector2 force = (((inner.transform.position - transform.position) * Vector2.right).normalized + Vector2.up) * 300f;
                inner.GetComponent<Player>().OnDamage(damage, force, .25f);
            }

            Destroy(gameObject);
        }
    }
}
