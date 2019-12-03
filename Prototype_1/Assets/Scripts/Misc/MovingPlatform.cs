using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private bool _isActivated;
    private Vector2 _startToEnd;
    private Vector2 _endToStart;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private Vector2 _currentDirection;
    private Transform _tr;
    private Transform _startEdge;
    private Transform _endEdge;
    private float _minSpeed;

    public float speed;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _startPos = _tr.parent.GetComponentInChildren<StartPos>().transform.position;
        _endPos = _tr.parent.GetComponentInChildren<EndPos>().transform.position;
        _startToEnd = (_endPos - _startPos).normalized;
        _endToStart = -_startToEnd;
        _currentDirection = _endToStart;
        _startEdge = GetComponentInChildren<PlatformStartEdge>().transform;
        _endEdge = GetComponentInChildren<PlatformEndEdge>().transform;
        _minSpeed = 2;
    }
    
    void FixedUpdate()
    {
        if (!_isActivated)
        {
            return;
        }

        UpdateDirection();

        Vector2 pos = transform.position;
        float distance = Math.Min(( _startPos - pos).magnitude, (_endPos - pos).magnitude);

        // Compute distance as a percentage having value 1.5 when the platform is in the middle of the moving path
        distance = distance * 3 / (_startPos - _endPos).magnitude;

        float currentSpeed = Math.Min(speed, speed * distance);
        if (currentSpeed < _minSpeed)
        {
            currentSpeed = _minSpeed;
        }
        
        Vector2 delta = currentSpeed * Time.fixedDeltaTime * _currentDirection;
        _tr.position += (Vector3) delta;
    }

    private void UpdateDirection()
    {
        if (_currentDirection == _endToStart && (_startPos - (Vector2) _startEdge.position).sqrMagnitude < 0.01f)
        {
            _currentDirection = _startToEnd;
            return;
        }

        if (_currentDirection == _startToEnd && (_endPos - (Vector2) _endEdge.position).sqrMagnitude < 0.01f)
        {
            _currentDirection = _endToStart;
        }
    }

    public void Activate()
    {
        _isActivated = true;
    }
}
