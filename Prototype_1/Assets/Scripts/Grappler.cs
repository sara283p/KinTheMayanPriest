using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    private CharacterController2D _controller;
    private Locker _locker;
    private Rigidbody2D _rb;
    private bool _hook;
    private DistanceJoint2D _joint;

    private void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _locker = GetComponent<Locker>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void SwitchToKinematic()
    {
        // To be implemented if we choose to use custom physics
    }

    private void HookInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _hook = true;
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            _hook = false;
        }
    }

    private void DestroyJoint()
    {
        _joint.connectedBody.gameObject.layer = 10;
        SwitchToKinematic();
        Destroy(_joint);
        _joint = null;
    }
    
    // Update is called once per frame
    void Update()
    {
        HookInput();
        // If the player is hanged to a star and either he's not pressing the hook button or he's touching the ground,
        // destroy the joint
        if (_joint != null)
        {
            if (!_hook || _controller.IsGrounded())
            {
                DestroyJoint();
            }
        }
        // If the player is locking a star, he's not touching the ground, he's pressing the hook button
        // and the joint does not exist yet, then create it
        else
        {
            if (_locker.IsLocked() && !_controller.IsGrounded() && _hook)
            {
                _joint = gameObject.AddComponent<DistanceJoint2D>();
                Rigidbody2D otherRb = _locker.GetTarget().GetComponent<Rigidbody2D>();
                _joint.distance = (_rb.position - otherRb.position).magnitude;
                _joint.connectedBody = otherRb;
                _locker.GetTarget().layer = 0;
            }
        }
    }
}
