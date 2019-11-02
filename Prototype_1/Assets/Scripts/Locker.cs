using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker : MonoBehaviour
{
	public GameObject activeIcon;
	
	private bool _locked;
	private GameObject _closest;
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
        	if (_closest != null)
        	{
        		activeIcon.transform.position = _closest.transform.position;
        		activeIcon.SetActive(true);
        		/*if (!_grounded)
        		{
        			if (_hook && _joint == null)
        			{
        				_joint = gameObject.AddComponent<DistanceJoint2D>();
        				Rigidbody2D otherRb = _closest.GetComponent<Rigidbody2D>();
        				_joint.distance = (rb2D.position - otherRb.position).magnitude;
        				_joint.connectedBody = otherRb;
        			}
        		}
        		else
        		{
        			if (_joint != null)
        			{
        				Destroy(_joint);
        				_joint = null;
        			}
        			SwitchToKinematic();
        			_hook = false;
        		}*/
        	}
        }
        else
        {
        	_closest = null;
        	activeIcon.SetActive(false);
        }

        FindClosestStar();
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
			    _closest = star;
		    }
	    }
    }

    public bool IsLocked()
    {
	    return _locked && _closest != null;
    }

    public GameObject GetTarget()
    {
	    return _closest;
    }
}
