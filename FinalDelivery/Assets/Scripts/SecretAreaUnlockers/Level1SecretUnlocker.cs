using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1SecretUnlocker : MonoBehaviour
{
    private CapsuleCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_collider.enabled)
        {
            EventManager.TriggerEvent("SecretUnlocked");
            enabled = false;
        }
    }
}
