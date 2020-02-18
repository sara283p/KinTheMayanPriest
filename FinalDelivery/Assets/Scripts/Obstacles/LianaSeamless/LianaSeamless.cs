using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaSeamless : Health, IDamageable
{
    private LianaSeamlessPiece[] _children;
    private Enemy[] _connectedEnemies;
    private float _maxHealth;
    private float _currentHealth;
    private float _aliveChildren;
    private Transform _targetPosition;

    public float fadeOutDurationInSeconds = 1f;

    void Awake()
    {
        _maxHealth = GameManager.Instance.obstacleMaxHealth;
        _currentHealth = _maxHealth;
        _targetPosition = GetComponentInChildren<LianaSeamlessTarget>().transform;
        _children = GetComponentsInChildren<LianaSeamlessPiece>();
        _connectedEnemies = GetComponentsInChildren<Enemy>(true);

        foreach (var enemy in _connectedEnemies)
        {
            enemy.gameObject.SetActive(false); 
        }
        
        GameManager.Instance.RegisterForRespawn(gameObject);
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
        foreach (var go in _children)
        {
            go.DestroyPiece();
        }

        GetComponent<Collider2D>().enabled = false;
        
        foreach (var enemy in _connectedEnemies)
        {
            enemy.gameObject.SetActive(true); 
        }
        
        Destroy(this);
    }

}
