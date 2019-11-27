using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;

    private float _horizontalMove = 0f;
    public float runSpeed = 300f;
    private bool _jump = false;
    private bool _crouch = false;
    private static readonly int Speed = Animator.StringToHash("Speed");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMove = InputManager.GetAxisRaw("Horizontal") * runSpeed;
        
        animator.SetFloat(Speed, Mathf.Abs(_horizontalMove));

        if (InputManager.GetButtonDown("Button0"))
        {
            _jump = true;
            //animator.SetBool("isJumping", true);
        }
        else if (InputManager.GetButtonUp("Button0"))
        {
            controller.StopJump();
        }

        /*if (Input.GetButtonDown("Crouch"))
        {
            _crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            _crouch = false;
        }*/

    }

    public void OnLanding()
    {
        //animator.SetBool("isJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        //animator.SetBool("isCrouching", isCrouching);
    }

    private void FixedUpdate()
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
        _jump = false;
    }
}