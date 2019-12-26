using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MovingPlatformSegmentEnd : MonoBehaviour
{
    protected CircleCollider2D coll;
    private MovingPlatform _platform;
    private bool _alreadyTriggered;
    private LayerMask _groundMask;

    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        _platform = transform.parent.parent.GetComponentInChildren<MovingPlatform>();
        _alreadyTriggered = true;
        _groundMask = LayerMask.GetMask("Ground");
    }

    protected abstract Collider2D[] GetEdgeColliders();

    public float GetDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _platform.transform.position - transform.position,
            (transform.position - _platform.transform.position).magnitude, _groundMask);
        return hit.distance;
    }

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