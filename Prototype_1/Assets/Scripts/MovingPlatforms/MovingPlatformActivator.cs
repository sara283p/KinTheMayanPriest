using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformActivator : MonoBehaviour, IDamageable
{
    private Transform _tr;
    private MovingPlatform _platform;
    private BoxCollider2D _coll;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _platform = _tr.parent.GetComponentInChildren<MovingPlatform>();
        _coll = GetComponent<BoxCollider2D>();
    }

    public void TakeDamage(float damage)
    {
        _platform.Activate();
        _coll.enabled = false;
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
