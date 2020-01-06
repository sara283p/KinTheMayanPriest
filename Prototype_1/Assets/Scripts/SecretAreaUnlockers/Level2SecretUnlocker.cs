using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2SecretUnlocker : MonoBehaviour
{
    public GameObject enigmaHideBarrier;
    
    private void OnDestroy()
    {
        if (enigmaHideBarrier) Destroy(enigmaHideBarrier);
    }
}
