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
    private bool _wantToHook;
    private bool _hook;
    private DistanceJoint2D _joint;

    private float _maxStarDistance = 5f;
    
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
            _wantToHook = true;
        }
        else if(Input.GetButtonUp("LockStarHang"))
        {
            _wantToHook = false;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            _hook = true;
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

        // If there is joint and either hook button is released or Kin is on the ground, destroy it
        if (_joint)
        {
            if (!_wantToHook || _controller.IsGrounded())
            {
                DestroyJoint();
                activeIcon.SetActive(false);
                hangingEffect.StopEffect();
            }
        }
        else
        {
            // If there is not a joint and hook is pressed...
            if (_wantToHook)
            {
                Star selectedStar = locker.GetTargetedStar();

                if (selectedStar)
                {
                    // First of all, illuminate the star
                    activeIcon.SetActive(true);
                    activeIcon.transform.position = selectedStar.transform.position;
                    
                    // If Kin is NOT the ground and meanwhile jump is pressed assume that the hang has to be accomplished.
                    // Create a joint, start the effect.
                    if (_hook && !_controller.IsGrounded() && ((Vector2) selectedStar.transform.position - _rb.position).magnitude < _maxStarDistance )
                    {
                        _joint = gameObject.AddComponent<DistanceJoint2D>();
                        Rigidbody2D otherRb = selectedStar.GetComponent<Rigidbody2D>();
                        _joint.distance = (_rb.position - otherRb.position).magnitude;
                        _joint.connectedBody = otherRb;
                        hangingEffect.StartEffect(otherRb.transform);
                    }
                }
            }
            // Hook has been released: de-illuminate the star.
            else
            {
                activeIcon.SetActive(false);
            }
        }
        _hook = false;
    }
}
