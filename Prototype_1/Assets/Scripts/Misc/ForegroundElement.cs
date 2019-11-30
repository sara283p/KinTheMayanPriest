using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundElement : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_renderer.isVisible)
        {
            _collider.enabled = true;
        }
        else
        {
            _collider.enabled = false;
        }
    }
}
