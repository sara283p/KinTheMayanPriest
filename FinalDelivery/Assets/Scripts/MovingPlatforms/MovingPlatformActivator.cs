using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformActivator : MonoBehaviour, IDamageable
{
    private Transform _tr;
    private MovingPlatform _platform;
    private int _groundLayer;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _platform = _tr.parent.GetComponentInChildren<MovingPlatform>();
        _groundLayer = LayerMask.NameToLayer("Ground");
    }

    public void TakeDamage(float damage)
    {
        _platform.Activate(gameObject);
        gameObject.layer = _groundLayer;
    }

    public Vector2 GetPosition()
    {
        return _tr.position;
    }

    public Transform GetTransform()
    {
        return _tr;
    }
}
