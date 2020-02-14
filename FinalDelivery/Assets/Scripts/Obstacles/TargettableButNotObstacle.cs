using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargettableButNotObstacle : MonoBehaviour, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;
    private float _aliveChildren;


    public void TakeDamage(float damage)
    {
        gameObject.SetActive(false);
    }

    public Vector2 GetPosition()
    {
        return (Vector2) transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsEnemy()
    {
        return false;
    }
}