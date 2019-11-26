using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Grappler_Joystick : MonoBehaviour
{
    public HangingEffect hangingEffect;
    public LayerMask obstacleLayerMask;
    
    private CharacterController _controller;
    public Locker_Joystick locker;
    private Rigidbody2D _rb;
    private bool _wantToHook;
    private DistanceJoint2D _joint;
    private FrictionJoint2D _friction;
    private bool _skyIsMoving;

    private float _maxStarDistance = 10f;
    
    private Star _selectedStar;
    private List<Star> _availableStars = new List<Star>();
    
    public MoveStarViewfinder_Joystick viewfinder;

    public Attack_Joystick attackJoystick;

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
        if (Input.GetAxis("LockStarHang") > 0.6f)
        {
            _wantToHook = true;
            attackJoystick.isHanging = true;

        }
        
        if(Input.GetAxis("LockStarHang") < 0.1f)
        {
            _wantToHook = false;
            attackJoystick.isHanging = false;
            
            _availableStars.ForEach( x => x.DeHighlightStar());   
            _availableStars.Clear();
            _selectedStar = null;
        }
        
        if (Input.GetButtonDown("RotateSky"))
        {
            _skyIsMoving = true;
        }
        else if (Input.GetButtonUp("RotateSky"))
        {
            _skyIsMoving = false;
        }
       
    }

    private void DestroyJoint()
    {
        _joint.connectedBody.gameObject.layer = 10;
        SwitchToKinematic();
        Destroy(_joint);
        Destroy(_friction);
        _joint = null;
        hangingEffect.StopEffect();

    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_rb.position, _selectedStar.GetComponent<Rigidbody2D>().position - _rb.position);
    }*/

    void Update()
    {
        HookInput();

        if (_selectedStar)
        {
            Vector2 relativePosition = _selectedStar.GetComponent<Rigidbody2D>().position - _rb.position;
            RaycastHit2D hit = Physics2D.Raycast(_rb.position, relativePosition, relativePosition.magnitude, obstacleLayerMask);
            if (hit.collider)
            {
                if (_joint)
                {
                    DestroyJoint();
                    _controller.ToggleHook();
                }
                return;
            }
        }
        else
        {
            _selectedStar = locker.GetTargetedStar();
        }
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
            
            if (!_wantToHook || _controller.IsGrounded() || _skyIsMoving)
            {
                DestroyJoint();
                _controller.ToggleHook();
            }
        }
        else
        {
            // If there is not a joint and hook is pressed...
            if (_wantToHook)
            {
                var temp = locker.GetStarsInRange(_maxStarDistance);
                _availableStars
                    .ForEach(x =>
                    {
                        if (!temp.Contains(x)) x.DeHighlightStar();
                    });

                _availableStars = temp;
                _availableStars.ForEach( x => x.HighlightStar());
                if (_availableStars.Count == 0 || !_selectedStar) return;
        
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
                    viewfinder.gameObject.transform.position = _selectedStar.transform.position;
                    
                    // If Kin is NOT the ground and meanwhile jump is pressed assume that the hang has to be accomplished.
                    // Create a joint, start the effect.
                    if (!_controller.IsGrounded() && ((Vector2) _selectedStar.transform.position - _rb.position).magnitude < _maxStarDistance )
                    {
                        _joint = gameObject.AddComponent<DistanceJoint2D>();
                        _friction = gameObject.AddComponent<FrictionJoint2D>();
                        Rigidbody2D otherRb = _selectedStar.GetComponent<Rigidbody2D>();
                        _joint.distance = (_rb.position - otherRb.position).magnitude;
                        _joint.connectedBody = otherRb;
                        _friction.maxForce = 1;
                        hangingEffect.StartEffect(otherRb.transform);
                        _controller.ToggleHook();
                    }
                }
            }
        }
    }

}
