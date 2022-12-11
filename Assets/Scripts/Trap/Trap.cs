using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{    
    [SerializeField] float damage;
    [SerializeField] float forceX;
    [SerializeField] float hitDelay;

    float timer = 0;

    private void Update() {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(timer <= 0)
        {
            if(other.gameObject.tag == "Player")
            {
                timer = 2;
                if(other.transform.position.x - transform.position.x > 0)
                {
                    other.gameObject.GetComponent<IDamageable>().OnDamage(damage, forceX * Vector2.right,hitDelay);
                } else other.gameObject.GetComponent<IDamageable>().OnDamage(damage, -forceX * Vector2.right,hitDelay);                
            }
        }
    }
}