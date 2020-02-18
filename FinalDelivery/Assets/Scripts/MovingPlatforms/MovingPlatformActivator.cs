using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformActivator : MonoBehaviour, IDamageable
{
    private Transform _tr;
    private MovingPlatform _platform;
    private int _groundLayer;
    private GameObject _outline;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _platform = _tr.parent.GetComponentInChildren<MovingPlatform>();
        _groundLayer = LayerMask.NameToLayer("Ground");
        _outline = GetComponentInChildren<ObstacleOutline>(true).gameObject;
    }

    public void TakeDamage(float damage)
    {
        if (_platform.Activate(gameObject))
        {
            gameObject.layer = _groundLayer;
        }
    }

    private void Update()
    {
        if (gameObject.layer == _groundLayer)
        {
            _outline.SetActive(false);
        }
        else
        {
            _outline.SetActive(true);
        }
    }

    public Vector2 GetPosition()
    {
        return _tr.position;
    }

    public Transform GetTransform()
    {
        return _tr;
    }

    public bool IsEnemy()
    {
        return false;
    }
}
