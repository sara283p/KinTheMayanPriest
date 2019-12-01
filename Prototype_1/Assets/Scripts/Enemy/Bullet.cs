using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float damageToPlayer = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageToPlayer);
        }
        //the bullet has to be destroyed no matter what it hit
        Destroy(gameObject);
    }
}
