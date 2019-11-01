using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    public GameObject impactEffect;

    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log(hitInfo.name);
            enemy.TakeDamage(damage);
        }

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}