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
    public ForceMode2D forcemode;
    
    //the waypoint the enemy is currently moving towards
    private int currentWaypoint = 0;
    
    [HideInInspector] public bool isPathOver = false;
    private bool isSearchingPlayer = false;
    
    //the max distance from a waypoint to trigger the enemy to move towards the next waypoint in the path
    public float nextWaypointDistance = 3f;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigidbody = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!isSearchingPlayer)
            {
                isSearchingPlayer = true;
                StartCoroutine(playerSearch());
            }
            
            return;
        }

        //search and begin a path starting from the enemy towards the target, and return the result to the function OnPathEnd()
        seeker.StartPath(transform.position, target.position, OnPathEnd);

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!isSearchingPlayer)
            {
                isSearchingPlayer = true;
                StartCoroutine(playerSearch());
            }
            
            yield return false;
        }
        
        seeker.StartPath(transform.position, target.position, OnPathEnd);
        
        yield return new WaitForSeconds(1f/updateRate);
        
        StartCoroutine(UpdatePath());
    }
    
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
        if (target == null)
        {
            if (!isSearchingPlayer)
            {
                isSearchingPlayer = true;
                StartCoroutine(playerSearch());
            }
            
            return;
        }

        if (foundPath == null)
        {
            return;
        }

        if (currentWaypoint >= foundPath.vectorPath.Count)
        {
            if (isPathOver)
            {
                return;
            }

            isPathOver = true;
            return;
        }

        isPathOver = false;

        Vector3 direction = (foundPath.vectorPath[currentWaypoint] - transform.position).normalized;
        direction *= enemySpeed * Time.fixedDeltaTime;
        rigidbody.AddForce(direction, forcemode);
            
        float distance = Vector3.Distance(transform.position, foundPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    IEnumerator playerSearch()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(playerSearch());
        }
        else
        {
            target = playerObject.transform;
            isSearchingPlayer = false;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }
}
