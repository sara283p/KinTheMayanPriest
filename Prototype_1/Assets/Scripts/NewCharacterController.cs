using System;
using UnityEngine;
using UnityEngine.Events;

public class NewCharacterController : MonoBehaviour

{
	[SerializeField] private float _jumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool _airControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask _whatIsGround;							// A mask determining what is ground to the character
	private ContactFilter2D _contactFilter;
	[SerializeField] private Transform _groundCheck;

	public Transform _leftJumpCheck;
	public Transform _rightJumpCheck;
	
	// A position marking where to check if the player is grounded.
	private float _oscillationSpeed;
	[SerializeField] private float _oscillationDelta;
	[SerializeField] private float _baseOscillationSpeed;
	private Vector2 _groundNormal;
	private Vector2 _currentNormal;
	private Vector2 _starPosition;
	private Collider2D[] _collArray = new Collider2D[1];
	
	private const float _groundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
	[SerializeField] private bool _grounded;            // Whether or not the player is grounded.
	private bool _hooked;
	private Rigidbody2D _rb;
	private Collider2D _collider;
	private bool _facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 _velocity = Vector3.zero;
	public Transform floorCheck;
	[SerializeField] private bool _jumping;
	
	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_collider = GetComponent<CircleCollider2D>();
		_contactFilter = new ContactFilter2D();
		_contactFilter.SetLayerMask(_whatIsGround);
		_contactFilter.useLayerMask = true;
		_groundNormal = new Vector2(0, 1);
	}

	private void FixedUpdate()
	{
		_grounded = false;
		
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		RaycastHit2D hit = Physics2D.Raycast(_leftJumpCheck.position, Vector2.down, 0.015f, _whatIsGround);
		Vector2 newPos = _rb.position;
		if (hit.collider)
		{
			_jumping = false;
		}
		else
		{
			hit = Physics2D.Raycast(_rightJumpCheck.position, Vector2.down, 0.015f, _whatIsGround);
			if (hit.collider)
			{
				_jumping = false;

			}
		}
		hit = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundedRadius, _whatIsGround);
		if (hit.collider)
		{
			_grounded = true;
		}
		hit = Physics2D.Raycast(floorCheck.position, Vector2.down, Mathf.Infinity, _whatIsGround);
		if (hit.collider)
		{
			_groundNormal = hit.normal;
		}
		else
		{
			_groundNormal = new Vector2(0, 1);
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
		Gizmos.DrawRay(_leftJumpCheck.position, 0.015f * Vector2.down);
		//Gizmos.DrawSphere(_groundCheck.position, _groundedRadius);
	}
	/*END OF GIZMOS SECTION */ 

	public void Move(float move, bool crouch, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (_grounded || _airControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity;
			if (!_hooked)
			{
				Vector2 moveAlongGround = new Vector2(_groundNormal.y, - _groundNormal.x);
				groundDir = moveAlongGround;   // Used for debugging purposes
				targetVelocity = new Vector2(move, _rb.velocity.y);
				if (!_jumping && !_grounded && targetVelocity.y > 0.01)
				{
					targetVelocity.y = -_rb.velocity.y;
					
				}
				// If player is falling or it is moving along a ramp, keep it's vertical speed
				/*if (!_grounded || _currentNormal == _groundNormal)
					targetVelocity.y = _rb.velocity.y;*/

				// And then smoothing it out and applying it to the character
			}
			else
			{
				targetVelocity = _rb.velocity;
				if(Math.Abs(move) > 0.01f)
						targetVelocity.x += Math.Sign(move) * _oscillationDelta;

				if (Math.Abs(targetVelocity.x) > _oscillationSpeed)
					targetVelocity.x = _oscillationSpeed * Math.Sign(targetVelocity.x);
			}
			
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
		if (_grounded && jump)
		{
				// If the player is not on a plain platform (avoids rocket jumps on ramps)
				if (_rb.velocity.y > 0.01)
					_rb.velocity = new Vector2(_rb.velocity.x, 0);
				_jumping = true;
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
		if (_hooked)
		{
			_starPosition = _rb.GetComponent<DistanceJoint2D>().connectedBody.position;
			_oscillationSpeed = _baseOscillationSpeed * (_rb.position - _starPosition).magnitude  * 0.75f;
		}
		else
		{
			_starPosition = Vector2.zero;
		}
	}
}
