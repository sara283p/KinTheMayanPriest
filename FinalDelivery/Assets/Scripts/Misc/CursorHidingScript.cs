using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHidingScript
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void OnBeforeSplashScreenLoadRuntimeMethod()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
}
