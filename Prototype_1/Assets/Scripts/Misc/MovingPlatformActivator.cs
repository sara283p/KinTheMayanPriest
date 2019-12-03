using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformActivator : MonoBehaviour, IDamageable
{
    private Transform _tr;
    private MovingPlatform _platform;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _platform = _tr.parent.GetComponentInChildren<MovingPlatform>();
    }

    public void TakeDamage(float damage)
    {
        _platform.Activate();
    }

    public Vector2 GetPosition()
    {
        return _tr.position;
    }
}
