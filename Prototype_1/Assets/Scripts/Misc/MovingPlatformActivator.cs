using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformActivator : MonoBehaviour, IDamageable
{
    private Transform _tr;
    private MovingPlatform _platform;
    public Transform activator;
    private Vector2 activatorPosition;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _platform = _tr.GetComponentInChildren<MovingPlatform>();
        activatorPosition = activator.position;
    }

    public void TakeDamage(float damage)
    {
        _platform.Activate();
    }

    public Vector2 GetPosition()
    {
        return activatorPosition;
    }

    public Transform GetTransform()
    {
        return _tr;
    }
}
