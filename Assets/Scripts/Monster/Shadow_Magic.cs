using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow_Magic : MonoBehaviourIF {
    public System.Action endEvent;

    public Transform target;

    float damage = 20;
    float moveSpeed = 3.5f;

    Vector2 dir;

    private void Start() {
        StartCoroutine("RemoveMagicOverTime");
    }
    private void Update() {
        MoveToTarget();
    }

    private void MoveToTarget() {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * (Mathf.Sin(Time.time) + 1));
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    private float MagicLookAtX(float x) {
        if (x > 0)
            transform.localScale = new Vector3(.5f, .5f, .5f);
        else if (x < 0)
            transform.localScale = new Vector3(-.5f, .5f, .5f);

        return transform.localScale.x;
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
