using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker : MonoBehaviour
{
	public GameObject activeIcon;
	public LayerMask starLayerMask;
	
	private bool _locked;
	private GameObject _target;
	private GameObject[] _stars;
	private float _distance;
	private Rigidbody2D _rb;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	private void LockInput()
	{
		if (Input.GetButtonDown("Fire3"))
		{
			_locked = true;
		}
		else if (Input.GetButtonUp("Fire3"))
		{
			_locked = false;
		}
	}
	
	// Update is called once per frame
    void Update()
    {
        LockInput();
	    if (_locked)
        {
        	if (_target != null)
        	{
        		activeIcon.transform.position = _target.transform.position;
        		activeIcon.SetActive(true);
            }
        }
        else
        {
        	_target = null;
        	activeIcon.SetActive(false);
        }

        //FindClosestStar();
        TargetStar();
    }
    
    private void FindClosestStar()
    {
	    _stars = GameObject.FindGameObjectsWithTag("Star");
	    _distance = Mathf.Infinity;
	    foreach (GameObject star in _stars)
	    {
		    float currDist = (_rb.position - star.GetComponent<Rigidbody2D>().position).magnitude;
		    if (currDist < _distance)
		    {
			    _distance = currDist;
			    _target = star;
		    }
	    }
    }

    private void TargetStar()
    {
	    // Get mouse position as a Vector2
	    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

	    // Cast a ray from player to mouse position
	    Vector2 relativeMousePos = mousePosition - _rb.position;
	    RaycastHit2D hit = Physics2D.Raycast(_rb.position, relativeMousePos, Mathf.Infinity, starLayerMask);

	    pos = relativeMousePos * 100;
	    if (hit.rigidbody == null)
	    {
		    activeIcon.SetActive(false);
		    return;
	    }

	    _target = hit.rigidbody.gameObject;
	}

    // START of GIZMOS section useful for debugging
    
    private Vector2 pos;

    private void OnDrawGizmos()
    {
	    if(pos != null)
			Gizmos.DrawRay(_rb.position, pos);
    }
    
    // END of GIZMOS section

    public bool IsLocked()
    {
	    return _locked && _target != null;
    }

    public GameObject GetTarget()
    {
	    return _target;
    }
}
