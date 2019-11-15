using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grappler_Joystick : MonoBehaviour
{
    public HangingEffect hangingEffect;
    private CharacterController2D _controller;
    public Locker_Joystick locker;
    private Rigidbody2D _rb;
    private bool _wantToHook;
    private bool _hook;
    private DistanceJoint2D _joint;

    private float _maxStarDistance = 10f;
    
    public GameObject activeIcon;

    private Star _selectedStar;
    private List<Star> _availableStars = new List<Star>();

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
            
            _availableStars = locker.GetStarsInRange(_maxStarDistance);
            _availableStars.ForEach( x => x.HighlightStar());
            _selectedStar = locker.GetNearestAvailableStar();
        }
        
        if(Input.GetButtonUp("LockStarHang"))
        {
            _wantToHook = false;
            
            _availableStars.ForEach( x => x.DeHighlightStar());   
            _availableStars.Clear();
            _selectedStar = null;
            activeIcon.SetActive(false);
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
                if (_availableStars.Count == 0) return;
        
                var pointedStar = locker.GetAvailableStarByRaycast(_selectedStar.transform);
                if (pointedStar)
                {
                    if (_availableStars.Contains(pointedStar))
                    {
                        _selectedStar = pointedStar;
                    }
                }

                if (_selectedStar)
                {
                    // First of all, illuminate the star
                    activeIcon.SetActive(true);
                    activeIcon.transform.position = _selectedStar.transform.position;
                    
                    // If Kin is NOT the ground and meanwhile jump is pressed assume that the hang has to be accomplished.
                    // Create a joint, start the effect.
                    if (_hook && !_controller.IsGrounded() && ((Vector2) _selectedStar.transform.position - _rb.position).magnitude < _maxStarDistance )
                    {
                        _joint = gameObject.AddComponent<DistanceJoint2D>();
                        Rigidbody2D otherRb = _selectedStar.GetComponent<Rigidbody2D>();
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
