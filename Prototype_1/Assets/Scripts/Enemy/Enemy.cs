using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Health, IDamageable
{
    private float _maxHealth;
    private float _curHealth;
    public float damageToPlayer;
    public int hitsToDeath;

    private Barrier _barrier;

    void Awake()
    {
        _maxHealth = GameManager.Instance.GetEnemyHealthFromHits(hitsToDeath);
        _curHealth = _maxHealth;

        try
        {
            _barrier = GetComponentInParent<Barrier>();
        }
        catch (Exception e)
        {
            _barrier = null;
        }
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

    public Transform GetTransform()
    {
        return transform;
    }

    void Die()
    {
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        if(_barrier != null) EventManager.TriggerEvent(string.Concat("DestroyBarrier", _barrier.GetInstanceID()));
        EventManager.TriggerEvent("EnemyKilled");
        
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