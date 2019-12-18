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
        _maxHealth = GameManager.Instance.obstacleMaxHealth;
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

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            DestroyObstacle();
        }
    }

    public Vector2 GetPosition()
    {
        foreach(Transform tr in transform)
        {
            if(tr.CompareTag("TargetPosition"))
            {
                return (Vector2) tr.position;
            }
        }
        return transform.position;
    }

    private void DestroyObstacle()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<StalattitePiece>())
            {
                child.GetComponent<CapsuleCollider2D>().enabled = true;
                child.gameObject.AddComponent<Rigidbody2D>();
                child.GetComponent<StalattitePiece>().Destroy();
            }
            
//            var solidColor = child.GetComponent<SpriteRenderer>().color;
//            var transparentColor = solidColor;
//            transparentColor.a = 0.3f;
//            child.GetComponent<SpriteRenderer>().material.color = solidColor;
//            child.GetComponent<SpriteRenderer>().material.color = transparentColor;
        }

        transform.DetachChildren();
        Destroy(gameObject);
        
    }
    
}
