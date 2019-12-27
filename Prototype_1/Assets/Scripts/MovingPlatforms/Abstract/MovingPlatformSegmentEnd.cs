using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MovingPlatformSegmentEnd : MonoBehaviour
{
    protected CircleCollider2D coll;
    private MovingPlatform _platform;
    protected bool alreadyTriggered;
    private LayerMask _groundMask;

    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        _platform = transform.parent.parent.GetComponentInChildren<MovingPlatform>();
        alreadyTriggered = true;
        _groundMask = LayerMask.GetMask("Ground");
    }

    protected abstract Collider2D[] GetEdgeColliders();

    public float GetDistance()
    {
        Vector2 pos = transform.position;
        Vector2 platformPos = _platform.transform.position;
        Vector2 relativePosition = platformPos - pos;
        
        RaycastHit2D hit = Physics2D.Raycast(pos, relativePosition, relativePosition.magnitude, _groundMask);
        return hit.distance;
    }

    private void Update()
    {
        Collider2D[] edgeColliders = Physics2D.OverlapCircleAll(transform.position, coll.radius)
            .Where(collid => collid.CompareTag("MovingPlatformEdge"))
            .ToArray();
        
        if (edgeColliders.Length <= 0)
        {
            alreadyTriggered = false;
            return;
        }
        
        if (alreadyTriggered)
            return;
        
        foreach (Collider2D collid in edgeColliders)
        {
            collid.transform.parent.GetComponent<MovingPlatform>().EndReached();
        }
        alreadyTriggered = true;
        

    }
}