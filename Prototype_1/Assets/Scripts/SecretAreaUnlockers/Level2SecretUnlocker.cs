using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2SecretUnlocker : MonoBehaviour
{
    private void OnDestroy()
    {
        EventManager.TriggerEvent("SecretUnlocked");
    }
    
}
