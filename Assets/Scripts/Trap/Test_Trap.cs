using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Trap : MonoBehaviour
{
    public Vector2 force = new Vector2(10, 0);
    
    public float minY,maxY;

    [Range(1,100)]
    public float speed;
    private int direction = -1;

    private void Update()
    {
        transform.position += new Vector3(0,speed * Time.deltaTime * direction,0);

        if(transform.position.y <= minY || transform.position.y >= maxY)
        {
            direction *= -1;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<IDamageable>().OnDamage(15f,force,2f);
        }
    }
}