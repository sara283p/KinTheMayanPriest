using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grappler_Joystick : MonoBehaviour
{
    public HangingEffect hangingEffect;
    public LayerMask obstacleLayerMask;
    
    private CharacterController _controller;
    public Locker_Joystick locker;
    private Rigidbody2D _rb;
    private Vector2 _position;
    private bool _wantToHook;
    private DistanceJoint2D _joint;
    private FrictionJoint2D _friction;
    private bool _skyIsMoving;
    [SerializeField] private bool _waitTillGrounded;

    private float _minHangDistance = 5;
    private float _maxStarDistance = 10f;
    private const float DownThresholdHang = 0.3f;
    private const float UpThresholdHang = 0.7f;
    
    private Star _selectedStar;
    private Star _nextStar;
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
        if (InputManager.GetAxis("RTrigger") > UpThresholdHang)
        {
            _wantToHook = true;
        }
        
        if(InputManager.GetAxis("RTrigger") < DownThresholdHang)
        {
            _wantToHook = false;
            
            _availableStars.ForEach( x => x.DeHighlightStar());   
            _availableStars.Clear();
            _selectedStar = null;
        }
        
//        if (InputManager.GetButtonDown("Button2"))
//        {
//            _skyIsMoving = true;
//        }
//        else if (InputManager.GetButtonUp("Button2"))
//        {
//            _skyIsMoving = false;
//        }
       
    }

    private void DestroyJoint()
    {
        attackJoystick.SetHanging(false);
        _joint.connectedBody.gameObject.layer = 10;
        SwitchToKinematic();
        Destroy(_joint);
        Destroy(_friction);
        _joint = null;
        hangingEffect.StopEffect();
        _selectedStar = null;

    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_position, _selectedStar.GetComponent<Rigidbody2D>().position - _position);
    }*/

    void Update()
    {
        _position = _rb.position;
        HookInput();

        if (_controller.IsGrounded())
        {
            _waitTillGrounded = false;
        }
        
        if (_wantToHook && !_selectedStar)
        {
            _selectedStar = locker.GetTargetedStar();
            _nextStar = _selectedStar;
        }

        if (_wantToHook && _selectedStar)
        {
            var target = locker.GetAvailableStarByRaycast(viewfinder.transform);
            if (target)
            {
                _nextStar = target;
                attackJoystick.AutoTargetWorking(false);
            }
            
            // Update viewfinder position
            viewfinder.gameObject.transform.position = _nextStar.transform.position;
        }

        if (_selectedStar)
        {
            Vector2 relativePosition = _selectedStar.GetComponent<Rigidbody2D>().position - _position;
            RaycastHit2D hit = Physics2D.Raycast(_position, relativePosition, relativePosition.magnitude, obstacleLayerMask);
            if (hit.collider)
            {
                if (_joint)
                {
                    DestroyJoint();
                    _waitTillGrounded = true;
                    _controller.ToggleHook();
                }
                return;
            }
        }
        // If there is joint and either hook button is released or Kin is on the ground, destroy it
        if (_joint)
        {
            Vector2 starCenteredRelativePosition = _position - _joint.connectedBody.position;
            
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

            if (!_wantToHook || _controller.IsGrounded()) // || _skyIsMoving)
            {
                _waitTillGrounded = false;
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

                if (_selectedStar)
                {
                    // If Kin is NOT the ground and meanwhile jump is pressed assume that the hang has to be accomplished.
                    // Create a joint, start the effect.
                    if (!_controller.IsGrounded() && ((Vector2) _selectedStar.transform.position - _position).magnitude < _maxStarDistance && !_waitTillGrounded)
                    {
                        attackJoystick.SetHanging(true);
                        _joint = gameObject.AddComponent<DistanceJoint2D>();
                        _friction = gameObject.AddComponent<FrictionJoint2D>();
                        Rigidbody2D otherRb = _selectedStar.GetComponent<Rigidbody2D>();
                        _joint.distance = (_position - otherRb.position).magnitude;
                        _joint.maxDistanceOnly = _joint.distance < _minHangDistance;
                        _joint.connectedBody = otherRb;
                        _joint.autoConfigureDistance = false;
                        _joint.enableCollision = true;
                        _friction.maxForce = 1;
                        hangingEffect.StartEffect(otherRb.transform);
                        _controller.ToggleHook();
                    }
                }
            }
        }
    }

}
