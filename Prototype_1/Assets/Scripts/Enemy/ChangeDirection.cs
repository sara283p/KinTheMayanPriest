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
        if (path.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (path.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
