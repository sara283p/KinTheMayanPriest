using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Health, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;

    private void Awake()
    {
        _maxHealth = 1;
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float GetHealth()
    {
        return _currentHealth;
    }

    public override float GetMaxHealth()
    {
        return _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            DestroyObstacle();
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    private void DestroyObstacle()
    {
        // Instantiate destruction animation
        Destroy(gameObject);
    }
}
