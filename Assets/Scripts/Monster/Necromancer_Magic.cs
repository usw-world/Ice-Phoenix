using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer_Magic : MonoBehaviourIF {
    public System.Action endEvent;

    float damage = 20;
    float moveSpeed = 5f;

    Vector2 dir;

    private void Start()
    {
        Initial();
        StartCoroutine("RemoveMagicOverTime");
    }
    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void Initial()
    {
        Transform targetTransform = GameObject.FindWithTag("Player").transform;
        dir = targetTransform.position - transform.position;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y * MagicLookAtX(dir.x), dir.x * MagicLookAtX(dir.x)) * Mathf.Rad2Deg, Vector3.forward);
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
