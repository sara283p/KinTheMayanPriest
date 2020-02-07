using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float damageToPlayer = 20f;

    [SerializeField] private float timeToLast = 3f;
    private float timeWhenGetDestroyed;

    // Start is called before the first frame update
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
        }
        Destroy(gameObject);
        
    }
}
