using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    //the target of the research
    public Transform target; 
    public float updateRate = 2f; 
    
    private Seeker seeker;
    private Rigidbody2D rigidbody;
    
    //variable used to save a found path towards the target
    private Path foundPath; 
    
    public float enemySpeed = 1000f;

    //the waypoint the enemy is currently moving towards
    private int currentWaypoint = 0;
    
    [HideInInspector] public bool isPathOver = false;

    //the max distance from a waypoint to trigger the enemy to move towards the next waypoint in the path
    public float nextWaypointDistance = 3f;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigidbody = GetComponent<Rigidbody2D>();

        //start iteratively searching for the quickest path towards the target
        InvokeRepeating("updatePath", 0f, .5f);
    }

    void updatePath()
    {
        if (seeker.IsDone())
        {
            //search and begin a path starting from the enemy towards the target, and return the result to the function OnPathEnd()
            seeker.StartPath(transform.position, target.position, OnPathEnd);
        }
    }

    //managing the behaviour upon the calculation of the quickest path (or not)
    public void OnPathEnd(Path path)
    {
        if (!path.error)
        {
            foundPath = path;
            currentWaypoint = 0;
        }
        else
        {
            Debug.LogError(path.error);
            return;
        }
    }

    void FixedUpdate()
    {
        if (foundPath == null)
        {
            return;
        }

        //if the enemy has reached the end of the path
        if (currentWaypoint >= foundPath.vectorPath.Count)
        {
            isPathOver = true;
            return;
        }
        
        else
        {
            isPathOver = false;
        }

        //apply the right force to move the enemy towards the path
        Vector2 direction = ((Vector2)foundPath.vectorPath[currentWaypoint] - rigidbody.position).normalized;
        Vector2 force = direction * enemySpeed * Time.deltaTime;
        rigidbody.AddForce(force);
            
        //if the enemy enters into the radius into which it's cvonsidered to be moving towards the next waypoint,
        //the waypoint counter is increased
        float distance = Vector2.Distance(transform.position, foundPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        
        //managing the way the enemy always faces the direction it's moving towards
        Vector3 newLocalScale = transform.localScale;
        if (force.x >= 0.01f)
        {
            newLocalScale.x = -Math.Abs(newLocalScale.x);
        }
        else if (force.x <= -0.01f)
        {
            newLocalScale.x = Math.Abs(newLocalScale.x);
        }
        transform.localScale = newLocalScale;

    }
}
