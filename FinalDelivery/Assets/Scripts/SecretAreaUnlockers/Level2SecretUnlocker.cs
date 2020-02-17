using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2SecretUnlocker : MonoBehaviour
{
    private Collider2D _barrierCollider;

    private void Awake()
    {
        _barrierCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!_barrierCollider.enabled)
        {
            EventManager.TriggerEvent("SecretUnlocked");
        }
    }

}
