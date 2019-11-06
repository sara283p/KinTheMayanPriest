using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    private float _leftBound;
    private float _rightBound;
    private float _extent;
    private BoxCollider2D _collider;
    private Rigidbody2D _rb;

    public float GetLeftBound()
    {
        return _leftBound;
    }

    public float GetRightBound()
    {
        return _rightBound;
    }

    public float GetExtent()
    {
        return _extent;
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        Bounds bounds = _collider.bounds;
        _extent = bounds.extents.x;
        _leftBound = _rb.position.x - _extent;
        _rightBound = _rb.position.x + _extent;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = _collider.bounds;
        float _center = bounds.center.x;
        _leftBound = bounds.min.x;
        _rightBound = bounds.max.x;
    }
}
