using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour

{
	private static readonly int Jumping = Animator.StringToHash("Jumping");
	
	public Transform _leftJumpCheck;											// Position marking used to check when the character has landed after jumps
	public Transform _rightJumpCheck;
	public Transform floorCheck;												// A position marking used to check the direction of the floor in front of the character

	[SerializeField] private float _jumpCheckRadius;							// Distance from ground at which the jump is considered to be ended
	[SerializeField] private float _jumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool _airControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask _whatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform _groundCheck;  							// A position marking where to check if the player is grounded.
	private float _oscillationSpeed;											// Max oscillation speed used to avoid circles while hanging
	[SerializeField] private float _oscillationDelta;							// Delta of space the player performs while oscillating
	[SerializeField] private float _baseOscillationSpeed;						// Base oscillation speed used to compute the maximum oscillation speed according to distance from the star
	private Vector2 _groundNormal;												// Vector in normal direction w.r.t. the ground
	private Vector2 _starPosition;												// Position of the star the player is hooked to
	private const float _groundedRadius = 0.2f; 								// Distance from ground at which player is considered to be grounded
	[SerializeField] private bool _grounded;            						// Whether or not the player is grounded.
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
	
	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_groundNormal = new Vector2(0, 1);
	}

	void Update()
	{
		_grounded = false;

		// Check if player has landed after jumping
		RaycastHit2D hit = Physics2D.Raycast(_leftJumpCheck.position, Vector2.down, _jumpCheckRadius, _whatIsGround);
		if (hit.collider)
		{
			_jumping = false;
			_animator.SetBool(Jumping, false);
		}
		else
		{
			hit = Physics2D.Raycast(_rightJumpCheck.position, Vector2.down, _jumpCheckRadius, _whatIsGround);
			if (hit.collider)
			{
				_jumping = false;
				_animator.SetBool(Jumping, false);
			}
		}
		// Check whether the player is on the ground
		Collider2D groundCollider = Physics2D.OverlapCircle(_groundCheck.position, _groundedRadius, _whatIsGround);
		if(groundCollider)
		{
			_grounded = true;
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
		Gizmos.DrawSphere(_groundCheck.position, _groundedRadius);
	}
	/*END OF GIZMOS SECTION */ 

	public void Move(float move, bool crouch, bool jump)
	{
		_rb.gravityScale = 1;
		//only control the player if grounded or airControl is turned on
		if (_grounded || _airControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity;
			// If the player is not hanged on a star...
			if (!_hooked)
			{
				Vector2 moveAlongGround = new Vector2(_groundNormal.y, - _groundNormal.x);
				groundDir = moveAlongGround;   // Used for debugging purposes

				// If the player is moving along a ramp...
				if (_grounded && !_jumping && Math.Abs(moveAlongGround.y) > 0.01)
				{
					targetVelocity = move * moveAlongGround;
					_rb.gravityScale = 0;
				}
				// ... otherwise...
				else
				{
					targetVelocity = new Vector2(move, _rb.velocity.y);
					
				}
				
				// If the player is not jumping, but the character is not grounded... (Reduces jumps at the end of ramps)
				if (!_jumping && !_grounded && _rb.velocity.y > - _antiRampJump)
				{
					Vector2 newVelocity = _rb.velocity;
					newVelocity.y = - _antiRampJump;
					_rb.velocity = newVelocity;
					targetVelocity.y = newVelocity.y;
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
					_animator.SetBool(Jumping, true);
					if (targetVelocity.y < -_maxVerticalSpeed)
					{
						targetVelocity.y = -_maxVerticalSpeed;
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
		}

		// If the player should jump...
		if (jump && (_grounded || Time.time < _coyoteTime))
		{
			_rb.gravityScale = 1;
			// Before applying the vertical force, re-initialize vertical speed to 0
			_rb.velocity = new Vector2(_rb.velocity.x, 0);
			_jumping = true;
			_startingJumpDirection = Math.Sign(move);
			_grounded = false;
			// Add a vertical force to the player.
			_rb.AddForce(new Vector2(0f, _jumpForce));
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
			_starPosition = _rb.GetComponent<DistanceJoint2D>().connectedBody.position;
			_oscillationSpeed = _baseOscillationSpeed * (_rb.position - _starPosition).magnitude * 0.8f ;
		}
		else
		{
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
}
