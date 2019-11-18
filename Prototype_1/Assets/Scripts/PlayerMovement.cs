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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        
        //animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
            //animator.SetBool("isJumping", true);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            controller.StopJump();
        }

        if (Input.GetButtonDown("Crouch"))
        {
            _crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            _crouch = false;
        }

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