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
    
    // This boolean flag is used to avoid that, if any obstacle comes in the grappler's ray, and therefore the grapple is destroyed, 
    // the player has the possibility to keep the trigger pressed to create another grapple. This is resetted when the player touches the ground
    [SerializeField] private bool _waitTillGrounded;

    private float _minHangDistance;
    private float _hangLengthVariationSpeed = 1;
    private float _maxHangDistance;
    private float _maxSelectDistance;
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
        _minHangDistance = GameManager.Instance.minHangDistance;
        _maxHangDistance = GameManager.Instance.maxHangDistance;
        _joint = GetComponent<DistanceJoint2D>();
        _friction = GetComponent<FrictionJoint2D>();
        _joint.autoConfigureDistance = false;
        _joint.enabled = false;
        _friction.enabled = false;
        _friction.enableCollision = true;
        _maxSelectDistance = GameManager.Instance.maxStarSelectDistance;
        enabled = GameManager.Instance.ShouldEnableGrappler();
    }

    private void SwitchToKinematic()
    {
        // To be implemented if we choose to use custom physics
    }

    private void HookInput()
    {
        if (InputManager.GetAxis("LTrigger") > UpThresholdHang)
        {
            _wantToHook = true;
        }
        
        if(InputManager.GetAxis("LTrigger") < DownThresholdHang)
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
        _joint.enabled = false;
        _friction.enabled = false;
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
            var target = locker.GetAvailableStarByRaycast(viewfinder.transform, _maxSelectDistance);
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
                if (_joint.enabled)
                {
                    DestroyJoint();
                    _waitTillGrounded = true;
                    _controller.ToggleHook();
                }
                return;
            }
        }
        // If there is joint and either hook button is released or Kin is on the ground, destroy it
        if (_joint.enabled)
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

            if (_joint.maxDistanceOnly && starCenteredRelativePosition.magnitude >= _minHangDistance - 0.01f)
            {
                _joint.maxDistanceOnly = false;
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
                var temp = locker.GetStarsInRange(_maxHangDistance);
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
                    if (!_controller.IsGrounded() && ((Vector2) _selectedStar.transform.position - _position).magnitude < _maxHangDistance && !_waitTillGrounded)
                    {
                        attackJoystick.SetHanging(true);
                        Rigidbody2D otherRb = _selectedStar.GetComponent<Rigidbody2D>();
                        _joint.connectedBody = otherRb;
                        _joint.distance = (_position - otherRb.position).magnitude;
                        _joint.maxDistanceOnly = _joint.distance < _minHangDistance;
                        if (_joint.maxDistanceOnly)
                            _joint.distance = _minHangDistance;
                        _friction.maxForce = 1;
                        _joint.enabled = true;
                        _friction.enabled = true;
                        hangingEffect.StartEffect(otherRb.transform);
                        _controller.ToggleHook();
                    }
                }
            }
        }
    }

}
