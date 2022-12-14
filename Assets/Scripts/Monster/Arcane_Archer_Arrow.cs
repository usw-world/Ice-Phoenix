using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arcane_Archer_Arrow: MonoBehaviourIF {
    public System.Action endEvent;

    float damage = 10f;
    float moveSpeed = 3f;

    Vector2 dir;

    private void Start()
    {
        StartCoroutine("RemoveMagicOverTime");
    }
    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.Translate(new Vector2(MagicLookAtX(dir.x), 0) * Time.deltaTime * moveSpeed);
    }

    private float MagicLookAtX(float x)
    {
        if (x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        return transform.localScale.x;
    }

    private IEnumerator RemoveMagicOverTime()
    {
        float removeTime = 5f;

        yield return new WaitForSeconds(removeTime);
        Destroy(gameObject);
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
