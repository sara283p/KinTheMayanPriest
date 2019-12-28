using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartPos : MovingPlatformSegmentEnd
{
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDeath", Init);
    }

    private void Init()
    {
        alreadyTriggered = true;
    }

    protected override Collider2D[] GetEdgeColliders()
    {
        return Physics2D.OverlapCircleAll(transform.position, coll.radius)
            .Where(collid => collid.GetComponent<PlatformStartEdge>())
            .ToArray();
    }
}
