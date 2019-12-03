using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Health, IDamageable
{
    private float _maxHealth;
    private float _curHealth;
    public float damageToPlayer = 20f;

    void Awake()
    {
        _maxHealth = GameManager.Instance.enemyMaxHealth;
        _curHealth = _maxHealth;
    }
    
    //public GameObject deathEffect;

    public void TakeDamage(float damage)
    {
        _curHealth -= damage;

        if (_curHealth <= 0)
        {
            Die();
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    void Die()
    {
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageToPlayer);
        }
    }

    public override float GetHealth()
    {
        return _curHealth;
    }

    public override float GetMaxHealth()
    {
        return _maxHealth;
    }
}