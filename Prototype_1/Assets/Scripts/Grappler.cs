using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public HangingEffect hangingEffect;
    private CharacterController _controller;
    public Locker_Joystick locker;
    private Rigidbody2D _rb;
    private bool _wantToHook;
    private bool _hook;
    private DistanceJoint2D _joint;
    private FrictionJoint2D _friction;

    private float _maxStarDistance = 5f;
    
    public GameObject activeIcon;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
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
        Destroy(_friction);
        _joint = null;
    }
    
    void Update()
    {
        HookInput();

        // If there is joint and either hook button is released or Kin is on the ground, destroy it
        if (_joint)
        {
            Vector2 starCenteredRelativePosition = _rb.position - _joint.connectedBody.position;
            
            // If the character is hanged to the star and is not moving, disable friction if remainder movement is very little, in 
            // order to make it do little oscillations around the center of the hanging ray...
            if (starCenteredRelativePosition.y <= -_joint.distance + 0.05f)
            {
                _friction.maxForce = 0;
            }
            // ... otherwise re-enable friction to slow character down when the player is not moving
            else
            {
                _friction.maxForce = 1f;
            }
            
            if (!_wantToHook || _controller.IsGrounded())
            {
                DestroyJoint();
                activeIcon.SetActive(false);
                hangingEffect.StopEffect();
                _controller.ToggleHook();
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
                        _friction = gameObject.AddComponent<FrictionJoint2D>();
                        Rigidbody2D otherRb = selectedStar.GetComponent<Rigidbody2D>();
                        _joint.distance = (_rb.position - otherRb.position).magnitude;
                        _joint.connectedBody = otherRb;
                        _friction.maxForce = 0.5f;
                        hangingEffect.StartEffect(otherRb.transform);
                        _controller.ToggleHook();
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
