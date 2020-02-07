using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingEnemy : MonoBehaviour
{
    private LayerMask _groundLayerMask;
    private bool _parentChanged;
    
    private void Awake()
    {
        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, _groundLayerMask);
        if (!_parentChanged && hit.collider)
        {
            Transform parent = hit.collider.transform;
            _parentChanged = true;
            transform.parent = parent;
        }
    }
}
