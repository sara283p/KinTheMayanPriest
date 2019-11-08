using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public HangingEffect hangingEffect;
    private CharacterController2D _controller;
    public Locker locker;
    private Rigidbody2D _rb;
    private bool _hook;
    private DistanceJoint2D _joint;
    
    public GameObject activeIcon;

    private void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void SwitchToKinematic()
    {
        // To be implemented if we choose to use custom physics
    }

    private void HookInput()
    {
        if (Input.GetButtonDown("LockStarHang"))
        {
            _hook = true;
        }
        else if(Input.GetButtonUp("LockStarHang"))
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
    
    void Update()
    {
        HookInput();
        
        if (_joint)
        {
            if (!_hook || _controller.IsGrounded())
            {
                DestroyJoint();
                activeIcon.SetActive(false);
                hangingEffect.StopEffect();
            }
        }
        else
        {
            if (_hook)
            {
                Star selectedStar = locker.GetTargetedStar();

                if (selectedStar)
                {
                    activeIcon.SetActive(true);
                    activeIcon.transform.position = selectedStar.transform.position;
                    if (!_controller.IsGrounded())
                    {
                        _joint = gameObject.AddComponent<DistanceJoint2D>();
                        Rigidbody2D otherRb = selectedStar.GetComponent<Rigidbody2D>();
                        _joint.distance = (_rb.position - otherRb.position).magnitude;
                        _joint.connectedBody = otherRb;
                        hangingEffect.StartEffect(otherRb.transform);
                    }
                }
            }
            else
            {
                activeIcon.SetActive(false);
            }
        }
    }
}
