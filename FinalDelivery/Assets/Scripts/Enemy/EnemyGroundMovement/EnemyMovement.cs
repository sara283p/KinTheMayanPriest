using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float moveSpeed = 3f;
    private Transform _leftWayPoint, _rightWayPoint;
    private bool _movingRight = true;
    private Rigidbody2D _rb;
    private Vector3 _unusedVelocity;
    
    // Start is called before the first frame update
    void Awake()
    {
        _unusedVelocity = Vector3.zero;
        _rb = GetComponent<Rigidbody2D>();
        _leftWayPoint = transform.parent.GetComponentInChildren<LeftWayPoint>().transform;
        _rightWayPoint = transform.parent.GetComponentInChildren<RightWayPoint>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= _rightWayPoint.position.x)
            _movingRight = false;
        else if (transform.position.x <= _leftWayPoint.position.x)
            _movingRight = true;

        if (_movingRight)
            MoveRight();
        else 
            MoveLeft();
    }


    void MoveRight()
    {
        Vector2 targetVelocity = new Vector2(moveSpeed, _rb.velocity.y);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _unusedVelocity, 0.05f);
    }
    
    void MoveLeft()
    {
        Vector2 targetVelocity = new Vector2( - moveSpeed, _rb.velocity.y);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _unusedVelocity, 0.05f);
    }
}
