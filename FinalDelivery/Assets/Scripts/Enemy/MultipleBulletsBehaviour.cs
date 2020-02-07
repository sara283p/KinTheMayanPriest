using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleBulletsBehaviour : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float damageToPlayer = 20f;

    public GameObject shooter;

    [SerializeField] private float timeToLast = 3f;
    private float timeWhenGetDestroyed;
    
    void Start()
    {
        rb.velocity = transform.up * speed;
        timeWhenGetDestroyed = Time.time + timeToLast;
    }

    private void Update()
    {
        if (Time.time >= timeWhenGetDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageToPlayer);
            Destroy(gameObject);
        }

        else if (other.CompareTag("Bullet") && other.isTrigger)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other.GetComponent<Collider>());
        }
        
        else if (other.CompareTag("Enemy") && other.isTrigger)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other.GetComponent<Collider>());
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
