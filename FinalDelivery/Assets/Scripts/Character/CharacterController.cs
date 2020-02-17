using System;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CharacterController : MonoBehaviour

{
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int IsPlayerTouching = Animator.StringToHash("IsPlayerTouching");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public bool drawGroundedSphere;
    public Transform _leftJumpCheck;											// Position marking used to check when the character has landed after jumps
    public Transform _rightJumpCheck;
    public Transform floorCheck;												// A position marking used to check the direction of the floor in front of the character
    public Sprite[] swingingSprites;
    public Sprite idleSprite;
    
    [SerializeField] private float _jumpCheckRadius;							// Distance from ground at which the jump is considered to be ended
    [SerializeField] private float _jumpForce = 400f;							// Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private LayerMask _whatIsGround;							// A mask determining what is ground to the character
    [SerializeField] private Transform _groundCheck;  							// A position marking where to check if the player is grounded.
    private float _oscillationSpeed;											// Max oscillation speed used to avoid circles while hanging
    [SerializeField] private float _oscillationDelta;							// Delta of space the player performs while oscillating
    [SerializeField] private float _baseOscillationSpeed;						// Base oscillation speed used to compute the maximum oscillation speed according to distance from the star
    private Vector2 _groundNormal;												// Vector in normal direction w.r.t. the ground
    private Vector2 _starPosition;												// Position of the star the player is hooked to
    [SerializeField] private float _groundedRadius = 0.2f; 						// Distance from ground at which player is considered to be grounded
    [SerializeField] private bool _grounded;            						// Whether or not the player is grounded.
    private bool _nearGround;
    private bool _hooked;														// Whether or not the player is hanged to a star
    private Rigidbody2D _rb;													// Character's rigidbody component
    private bool _facingRight = true; 											// For determining which way the player is currently facing.
    private Vector3 _velocity = Vector3.zero;									// Velocity computed by SmoothDamp method
    [SerializeField] private bool _jumping;										// Whether or not the player has jumped
    [SerializeField] private int _startingJumpDirection;						// Right -> 1; Left -> -1; None -> 0
    [SerializeField] private float _coyoteDuration = 0.2f;
    private float _coyoteTime;
    [SerializeField] private float _minVerticalSpeed = 1;
    [SerializeField] private float _maxVerticalSpeed = 25;
    [SerializeField] private float _antiRampJump = 5;
    private Animator _animator;
    private CapsuleCollider2D _collider;
    public LayerMask obstacleLayerMask;
    private bool _isInWater;
    private bool _isInLava;
    private float _lavaDamage;
    private float _waterSpeedModifier;
    private float _waterGravityModifier;
    private float _lavaSpeedModifier;
    private float _lavaGravityModifier;
    private Vector2 _wallPosition;                                                // Vector used to identify the position of the touched wall w.r.t. Kin in order to avoid sticky character
    private PlayerHealth _health;
    private SpriteRenderer _spriteRenderer;

    private Transform _initialParent;
    private bool _justReleasedHook;
	
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundNormal = new Vector2(0, 1);
        _initialParent = transform.parent;
        _collider = GetComponents<CapsuleCollider2D>()
            .First(coll => !coll.isTrigger);
        _lavaDamage = GameManager.Instance.lavaDamage;
        _waterSpeedModifier = GameManager.Instance.waterSpeedModifier;
        _waterGravityModifier = GameManager.Instance.waterGravityModifier;
        _lavaSpeedModifier = GameManager.Instance.lavaSpeedModifier;
        _lavaGravityModifier = GameManager.Instance.lavaGravityModifier;
        _health = GetComponent<PlayerHealth>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _grounded = false;
        _nearGround = false;

        // Check if player has landed after jumping
        RaycastHit2D hit = Physics2D.Raycast(_leftJumpCheck.position, Vector2.down, _jumpCheckRadius, _whatIsGround);
        Vector2 rayDir = _leftJumpCheck.position;
        rayDir.y -= _jumpCheckRadius;
        Collider2D coll = Physics2D.OverlapPoint(rayDir, _whatIsGround);
        if (coll || hit.collider)
        {
            _jumping = false;
            _nearGround = true;
        }
        else
        {
            hit = Physics2D.Raycast(_rightJumpCheck.position, Vector2.down, _jumpCheckRadius, _whatIsGround);
            rayDir = _rightJumpCheck.position;
            rayDir.y -= _jumpCheckRadius;
            coll = Physics2D.OverlapPoint(rayDir, _whatIsGround);
            if (coll || hit.collider)
            {
                _jumping = false;
                _nearGround = true;
            }
        }

        // Check whether the player is on the ground
        Collider2D groundCollider = Physics2D.OverlapCircle(_groundCheck.position, _groundedRadius, _whatIsGround);
        if(groundCollider)
        {
            _grounded = true;
            _justReleasedHook = false;
            
            // To avoid remaining child of the moving platform in the frame the player jumps, set the parent to be the moving platform only if he did not jump
            if (!_jumping)
            {
                transform.SetParent(groundCollider.CompareTag("MovingPlatform")
                    ? groundCollider.transform
                    : _initialParent);
				
                _animator.SetBool(Jumping, false);
            }
            _animator.SetBool(Falling, false);
        }

        // Compute floor direction
        hit = Physics2D.Raycast(floorCheck.position, Vector2.down, Mathf.Infinity, _whatIsGround);
        if (hit.collider)
        {
            _groundNormal = hit.normal;
        }
        else
        {
            _groundNormal = new Vector2(0, 1);
        }

        if (_grounded)
        {
            _coyoteTime = Time.time + _coyoteDuration;
        }

        if (_isInLava)
        {
            _health.TakeDamage(_lavaDamage);
        }
    }


    /* START OF GIZMOS SECTION*/
    public Vector2 groundDir;
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(_rb.position, groundDir);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(floorCheck.position, Vector2.down * Mathf.Infinity);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_leftJumpCheck.position, _jumpCheckRadius * Vector2.down);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_rightJumpCheck.position, _jumpCheckRadius * Vector2.down);
        //Gizmos.DrawRay(_groundCheck.position, _groundedRadius * Vector2.down);
        if(drawGroundedSphere)
            Gizmos.DrawSphere(_groundCheck.position, _groundedRadius);
    }
    /*END OF GIZMOS SECTION */ 

    public void Move(float move, bool crouch, bool jump)
    {
        _rb.gravityScale = 1;
        // Move the character by finding the target velocity
        Vector3 targetVelocity;
        // If the player is not hanged on a star...
        if (!_hooked)
        {
            _animator.SetFloat(Speed, Math.Abs(move));
            if (_isInWater)
            {
                move *= _waterSpeedModifier;
                _rb.gravityScale = _waterGravityModifier;
            }

            if (_isInLava)
            {
                move *= _lavaSpeedModifier;
                _rb.gravityScale = _lavaGravityModifier;
            }
				
            Vector2 moveAlongGround = new Vector2(_groundNormal.y, - _groundNormal.x);
            groundDir = moveAlongGround;   // Used for debugging purposes

            // If the player is moving along a ramp...
            if (_grounded && !_jumping && Math.Abs(moveAlongGround.y) > 0.01)
            {
                targetVelocity = move * moveAlongGround;
                if(_nearGround)
                    _rb.gravityScale = 0;
            }
            // ... otherwise...
            else
            {
                targetVelocity = new Vector2(move, _rb.velocity.y);
					
            }
				
            // If the player is not jumping, but the character is not grounded... (Reduces jumps at the end of ramps)
            if (!_jumping && !_grounded && !_isInWater && !_isInLava && !_justReleasedHook &&  _rb.velocity.y > - _antiRampJump)
            {
                Vector2 newVelocity = _rb.velocity;
                newVelocity.y = - _antiRampJump;
                _rb.velocity = newVelocity;
                targetVelocity.y = newVelocity.y;
                _rb.gravityScale = 1;
            }

            // If the player jumped
            if (_jumping)
            {
                Vector2 newVelocity = _rb.velocity;
				
                // When the vertical velocity drops down a certain threshold, just set it to a minimum value to increase starting falling speed
                if(newVelocity.y < 2 && newVelocity.y > - _minVerticalSpeed)
                {
                    newVelocity.y = -_minVerticalSpeed;
                    _rb.velocity = newVelocity;
                    targetVelocity.y = newVelocity.y;
                }

                // If player changes direction, reduce air control of the character
                /*if (targetVelocity.x * newVelocity.x < 0)
                {
                    if (Math.Abs(newVelocity.x) > 0.5f)
                    {
                        targetVelocity.x = newVelocity.x * 0.8f;
                    }
                    else
                    {
                        targetVelocity.x += 0.5f * Math.Sign(targetVelocity.x);
                    }
                }

                // If character horizontal move drops to 0 (e.g. changes direction while on air), permanently reduce air control
                if (Math.Abs(targetVelocity.x) <= 0.01)
                {
                    _startingJumpDirection = 0;
                }
                
                // If player jumps without moving or the character changed direction while on air, reduce air control
                if (_startingJumpDirection == 0)
                {
                    targetVelocity.x = 0.5f * targetVelocity.x;
                }*/
            }

            if (!_grounded)
            {
                _animator.SetBool(Falling, true);
                if (targetVelocity.y < -_maxVerticalSpeed)
                {
                    targetVelocity.y = -_maxVerticalSpeed;
                }

                if (_collider.IsTouchingLayers(obstacleLayerMask))
                {
                    if(_wallPosition == Vector2.zero)
                        _wallPosition = new Vector2(Math.Sign(targetVelocity.x), 0);
						
                    Debug.DrawRay(_rb.position, _wallPosition, Color.green);
						
                    if (targetVelocity.x * _wallPosition.x > 0)
                    {
                        targetVelocity.x = 0;
                    }
                }
                else
                {
                    _wallPosition = Vector2.zero;
                }
            }
        }
        // ... otherwise, if player is hanged to a star...
        else
        {
            targetVelocity = _rb.velocity;
            // If player is moving horizontally, increase oscillation... (second term of the conjunction is needed to avoid stalls when the player keeps a button pressed.
            // E.g. if player keeps "D" pressed, character won't stall to the right while hanged to the star)
            if(Math.Abs(move) > 0.01f && !(targetVelocity.y < 0 && targetVelocity.x * move < 0 && Math.Abs(targetVelocity.x) < 2))
                targetVelocity.x += Math.Sign(move) * _oscillationDelta;
				
            //... but if oscillation speed is too high, set it to a maximum speed to avoid performing entire circles
            if (Math.Abs(targetVelocity.x) > _oscillationSpeed)
                targetVelocity.x = _oscillationSpeed * Math.Sign(targetVelocity.x);

            // Also check vertical velocity and when the character vertical position is near the hanged star vertical position
            // and he's going up just reduce the velocity. This is also done in order to reduce possibility to do entire circles
            // around the star
            Vector2 starRelativePosition = _starPosition - _rb.position;
            if (Math.Abs(starRelativePosition.y) < 0.3f && targetVelocity.y > 0)
            {
                targetVelocity.y = targetVelocity.y * 0.7f;
            }

            float minSwingSpeed = _oscillationSpeed / 3;
            float facingDir = transform.localRotation.y;
            
            // To avoid having facingDir = 0, and so direction = 0, if localRotation.y is 0, set it to 1 (because it means
            // character is facing right)
            if (Math.Abs(transform.localRotation.y) <= 0.01)
            {
                facingDir = 1;
            }
            int direction = Math.Sign(targetVelocity.x * facingDir);
            
            if (Mathf.Abs(targetVelocity.x) <= minSwingSpeed)
            {
                _spriteRenderer.sprite = swingingSprites[0];
            }
            else if (Mathf.Abs(targetVelocity.x) > _oscillationSpeed - minSwingSpeed)
            {
                _spriteRenderer.sprite = direction >= 0 ? swingingSprites[2] : swingingSprites[4];
            }
            else
            {
                _spriteRenderer.sprite = direction >= 0 ? swingingSprites[1] : swingingSprites[3];
            }

        }
			
        // Set rigidbody velocity to move player
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, m_MovementSmoothing);


        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !_facingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && _facingRight)
        {
            // ... flip the player.
            Flip();
        }

        // If the player should jump...
        if (jump && !_jumping && (_grounded || Time.time < _coyoteTime))
        {
            _rb.gravityScale = 1;
            // Before applying the vertical force, re-initialize vertical speed to 0
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _jumping = true;
            _startingJumpDirection = Math.Sign(move);
            _grounded = false;
            transform.SetParent(_initialParent);
            // Add a vertical force to the player.
            float forceMagnitude = _isInWater ? _jumpForce * _waterSpeedModifier : _jumpForce;
            forceMagnitude = _isInLava ? _jumpForce * _lavaSpeedModifier : forceMagnitude;
            _animator.SetBool(Jumping, true);
            _rb.AddForce(new Vector2(0f, forceMagnitude));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    public void ToggleHook()
    {
        _hooked = !_hooked;
        // If player is hanging to a star, set the maximum oscillation speed according to the distance from the star
        if (_hooked)
        {
            _animator.enabled = false;
            _spriteRenderer.sprite = swingingSprites[0];
            DistanceJoint2D joint = _rb.GetComponent<DistanceJoint2D>();
            _starPosition = joint.connectedBody.position;
            _oscillationSpeed = _baseOscillationSpeed * joint.distance * 0.8f ;
        }
        else
        {
            _spriteRenderer.sprite = idleSprite;
            _animator.enabled = true;
            _justReleasedHook = true;
            _starPosition = Vector2.zero;
        }
    }

    public void StopJump()
    {
        Vector2 velocity = _rb.velocity;
        if (_jumping && velocity.y > 0)
        {
            velocity.y *= 0.5f;
            _rb.velocity = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lava"))
        {
            _isInLava = true;
            Vector2 currentVelocity = _rb.velocity;
            currentVelocity.y = _lavaSpeedModifier * Math.Sign(currentVelocity.y);
            _rb.velocity = currentVelocity;
            return;
        }

        if (other.CompareTag("Water"))
        {
            _isInWater = true;
            Vector2 currentVelocity = _rb.velocity;
            currentVelocity.y = _waterSpeedModifier * Math.Sign(currentVelocity.y);
            _rb.velocity = currentVelocity;
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Lava"))
        {
            _isInLava = false;
            return;
        }

        if (other.CompareTag("Water"))
        {
            _isInWater = false;
            return;
        }
    }
}