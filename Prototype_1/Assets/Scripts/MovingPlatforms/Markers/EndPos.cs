using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndPos : MovingPlatformSegmentEnd
{
    protected override Collider2D[] GetEdgeColliders()
    {
        return Physics2D.OverlapCircleAll(transform.position, coll.radius)
            .Where(collid => collid.GetComponent<PlatformEndEdge>())
            .ToArray();
    }
}
