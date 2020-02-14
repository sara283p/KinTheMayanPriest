using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : Health, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;
    private float _aliveChildren;
    private Transform _targetPosition;
    private AudioManager _audioManager;
    private bool _usedForEnigma;
    
    private void Awake()
    {
        _audioManager = AudioManager.instance;
        _maxHealth = GameManager.Instance.obstacleMaxHealth;
        _currentHealth = _maxHealth;
        _aliveChildren = 0;
        _targetPosition = GetComponentInChildren<StalactiteTarget>().transform;
        _usedForEnigma = GetComponentInParent<Enigma>();
        
        if(!_usedForEnigma)
            GameManager.Instance.RegisterForRespawn(gameObject);
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
        return _targetPosition.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsEnemy()
    {
        return false;
    }

    private void DestroyObstacle()
    {
        if (_usedForEnigma)
        {
            GetComponent<SequentialDestructible>().enabled = false;
        }
        EventManager.TriggerEvent("TargetDestroyed");
        _audioManager.Play("StalattiteBreak");
        GetComponent<CapsuleCollider2D>().enabled = false;
        ObstacleOutline outline = GetComponentInChildren<ObstacleOutline>();
        if (outline)
        {
            Destroy(outline.gameObject);
        }

        foreach (var child in transform.GetComponentsInChildren<StalattitePiece>())
        {
            _aliveChildren++;
            child.gameObject.AddComponent<Rigidbody2D>();
            child.Destroy();

//            var solidColor = child.GetComponent<SpriteRenderer>().color;
//            var transparentColor = solidColor;
//            transparentColor.a = 0.3f;
//            child.GetComponent<SpriteRenderer>().material.color = solidColor;
//            child.GetComponent<SpriteRenderer>().material.color = transparentColor;
        }
    }

    public void OnChildrenDestroyed()
    {
        _aliveChildren--;
        if (_aliveChildren <= 0)
        {
            Destroy(gameObject);
        }
    }
    
}