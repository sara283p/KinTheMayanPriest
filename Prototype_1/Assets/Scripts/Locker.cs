using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker : MonoBehaviour
{
	public GameObject activeIcon;
	public LayerMask starLayerMask;

	private Vector2 _oldMousePosition;
	private bool _locked;
	private GameObject _target;
	private GameObject[] _stars;
	private float _distance;
	private Rigidbody2D _rb;

	public Transform starViewFinder;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	private void LockInput()
	{
		if (Input.GetButtonDown("LockStarHang"))
		{
			_locked = true;
		}
		else if (Input.GetButtonUp("LockStarHang"))
		{
			_locked = false;
		}
	}
	
	// Update is called once per frame
    void Update()
    {
        LockInput();
	    
        //FindClosestStar();
        TargetStar();
    }
    
    
    private void TargetStar()
    {
	    // Get mouse position as a Vector2
	    Vector2 mousePosition = starViewFinder.position;

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
//
//    private void OnDrawGizmos()
//    {
//	    if(pos != null)
//			Gizmos.DrawRay(_rb.position, pos);
//    }
//    
    // END of GIZMOS section

    public bool IsLocked()
    {
	    return _locked;
    }

    public GameObject GetTarget()
    {
	    return _target;
    }
}
