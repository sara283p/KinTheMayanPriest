using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovement : MonoBehaviour
{
    public NewCharacterController controller;
    public Animator animator;

    private float horizontalMove = 0f;
    private float runSpeed = 400f;
    private bool jump = false;
    private bool crouch = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        
        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            //animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
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
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}