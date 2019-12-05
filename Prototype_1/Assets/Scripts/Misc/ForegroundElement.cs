using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForegroundElement : MonoBehaviour
{
    private TilemapCollider2D _collider;
    private Renderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<TilemapCollider2D>();
        _renderer = GetComponent<Renderer>();
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
