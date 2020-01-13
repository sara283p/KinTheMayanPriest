using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ChangeDirection : MonoBehaviour
{
    public AIPath path;
    
    //rotate the enemy sprite when changing direction
    void Update()
    {
        Vector3 newLocalScale = transform.localScale;
        if (path.desiredVelocity.x >= 0.01f)
        {
            newLocalScale.x = -Math.Abs(newLocalScale.x);
        }
        else if (path.desiredVelocity.x <= -0.01f)
        {
            newLocalScale.x = Math.Abs(newLocalScale.x);
        }
    }
}
