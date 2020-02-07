using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public float horizontalInput;
    public bool jumpPressed;
    public bool jumpHeld;
    public bool crouchPressed;
    public bool crouchHeld;

    public bool readyToClear;

    void Update()
    {
        ClearInputs();

        processInputs();

        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
    }

    //given that FixedUpdate is called before Update, we set readyToClear true right before it to "clean" the
    //previous inputs and prepare to process newer ones
    private void FixedUpdate()
    {
        readyToClear = true;
    }


    //reset all input values stored
    void ClearInputs()
    {
        if (!readyToClear)
        {
            return;
        }

        horizontalInput = 0f;
        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;
        readyToClear = false;
    }


    //save all input values
    void processInputs()
    {
        horizontalInput += Input.GetAxis("Horizontal");

        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");
        crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
        crouchHeld = crouchHeld || Input.GetButton("Crouch");
    }
}
