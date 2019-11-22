using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlayerMovement : MonoBehaviour
{
    [Header("Movement related")]
    public float runSpeed = 10f;
    public float crouchSpeedDivisor = 3f;  //reduction applied to movement speed when crouched
    public float coyoteDuration = .05f;  //how long after going off a platform the player can still jump
    public float maxFallSpeed = -25f;

    [Header("Jump related")] 
    public float jumpForce = 3f; //initial force applied for the jump
    public float holdingJumpForce = 1f; //additional force added to the jump if pression is kept
    public float maxJumpHoldingTime = .1f; //max time the jump button can be pressed  

    [Header("Booleans")]
    public bool isCrouching; 
    public bool isJumping;
    public bool isOnGround;

    [Header("Player Related")]
    private PlayerInput input;
    [SerializeField] CapsuleCollider2D bodyCollider;
    Rigidbody2D rigidBody;
    private float originalXScale;
    public float footOffset = .4f;
    public float groundDistance = 1.2f;

    [Header("Colliders Standing-Crouched")]
    Vector2 colliderStandSize;				
    Vector2 colliderStandOffset;			
    Vector2 colliderCrouchSize;				
    Vector2 colliderCrouchOffset;

    [Header("Other values")]
    public bool drawDebugRaycasts = true;  //if we want to see the raycasts
    public LayerMask groundLayer;
    private float jumpTime;
    private float coyoteTime;
    public int direction = 1; //1 if directed towards the positive x axis, -1 if directed towards the negative x axis

    private bool _hooked;														// Whether or not the player is hanged to a star
    private Vector2 _starPosition;												// Position of the star the player is hooked to
    private float _oscillationSpeed;											// Max oscillation speed used to avoid circles while hanging
    [SerializeField] private float _baseOscillationSpeed;						// Base oscillation speed used to compute the maximum oscillation speed according to distance from the star

    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        originalXScale = transform.localScale.x;

        colliderStandSize = bodyCollider.size;
        colliderStandOffset = bodyCollider.offset;
        colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2f);
        colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2);
    }

    private void FixedUpdate()
    {
        physicsCheck();
        groundMovement();
        airMovement();
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(input.horizontalInput*runSpeed));
    }

    void physicsCheck()
    {
        isOnGround = false;

        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance);

        if (leftCheck || rightCheck)
        {
            isOnGround = true;
        }
    }

    void groundMovement()
    {
        //if "Crouch" is pressed while the player on the ground and isn't already crouching, it crouches
        if (input.crouchHeld && !isCrouching && isOnGround)
        {
            crouch();
        }
        //else if the player is crouching but the "Crouch" button is not pressed, the player stands up
        else if (!input.crouchHeld && isCrouching)
        {
            standUp();
        }
        else if (!isOnGround && isCrouching)
        {
            standUp();
        }

        float xSpeed = runSpeed * input.horizontalInput;

        //if the characters changes direction we adjust its scale to rotate him
        if (xSpeed * direction < 0f)
        {
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x = originalXScale * direction;
            transform.localScale = scale;
        }

        if (isCrouching)
        {
            xSpeed /= crouchSpeedDivisor;
        }

        rigidBody.velocity = new Vector2(xSpeed, rigidBody.velocity.y);

        if (isOnGround)
        {
            coyoteTime = Time.time + coyoteDuration;
        }
    }

    void airMovement()
    {
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time))
        {
            isOnGround = false;
            isJumping = true;

            //saving the time at which the player won't be able to improve its jump height
            jumpTime = Time.time + maxJumpHoldingTime;

            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        else if (isJumping)
        {
            if (input.jumpHeld)
            {
                rigidBody.AddForce(new Vector2(0f, holdingJumpForce), ForceMode2D.Impulse);
            }

            if (jumpTime <= Time.time)
            {
                isJumping = false;
            }
        }

        if (rigidBody.velocity.y < maxFallSpeed)
        {
             rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
        }
    }

    void crouch()
    {
        isCrouching = true;

        bodyCollider.size = colliderCrouchSize;
        bodyCollider.offset = colliderCrouchOffset;
    }

    void standUp()
    {
        isCrouching = false;

        bodyCollider.size = colliderStandSize;
        bodyCollider.offset = colliderStandOffset;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    	{
            return Raycast(offset, rayDirection, length, groundLayer);
    	}

    	RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    	{
            Vector2 pos = transform.position;

    		RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);


    		if (drawDebugRaycasts)
    		{
    			//...determine the color based on if the raycast hit...
    			Color color = hit ? Color.red : Color.green;
    			//...and draw the ray in the scene view
    			Debug.DrawRay(pos + offset, rayDirection * length, color);
    		}

    		//Return the results of the raycast
    		return hit;
    	}
        
        public void ToggleHook()
        {
            _hooked = !_hooked;
            // If player is hanging to a star, set the maximum oscillation speed according to the distance from the star
            if (_hooked)
            {
                _starPosition = rigidBody.GetComponent<DistanceJoint2D>().connectedBody.position;
                _oscillationSpeed = _baseOscillationSpeed * (rigidBody.position - _starPosition).magnitude  * 0.75f;
            }
            else
            {
                _starPosition = Vector2.zero;
            }
        }
        
        public bool IsGrounded()
        {
            return isOnGround;
        }
}