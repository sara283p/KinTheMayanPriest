using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomBaseInput : BaseInput
{
    public override float GetAxisRaw(string axisName)
    {
        return InputManager.GetAxisRaw(axisName);
    }

    public override bool GetButtonDown(string buttonName)
    {
        return InputManager.GetButtonDown(buttonName);
    }

    public override bool mousePresent => false;
}
