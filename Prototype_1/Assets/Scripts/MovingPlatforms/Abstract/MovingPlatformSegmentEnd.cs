using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MovingPlatformSegmentEnd : MonoBehaviour
{
    protected CircleCollider2D coll;
    private bool _alreadyTriggered;

    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        _alreadyTriggered = true;
    }

    protected abstract Collider2D[] GetEdgeColliders();

    private void Update()
    {
        Collider2D[] edgeColliders = Physics2D.OverlapCircleAll(transform.position, coll.radius)
            .Where(collid => collid.CompareTag("MovingPlatformEdge"))
            .ToArray();
        
        if (edgeColliders.Length <= 0)
        {
            _alreadyTriggered = false;
            return;
        }
        
        if (_alreadyTriggered)
            return;
        
        foreach (Collider2D collid in edgeColliders)
        {
            collid.transform.parent.GetComponent<MovingPlatform>().EndReached();
        }
        _alreadyTriggered = true;
        

    }
}