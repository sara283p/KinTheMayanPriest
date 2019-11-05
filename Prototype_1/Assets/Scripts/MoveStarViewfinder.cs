﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStarViewfinder : MonoBehaviour
{
    public LayerMask starLayerMask;
    public LayerMask enemyLayerMask;
    
    private Vector2 _newMousePosition;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private float moveSpeed = 20f;
    private Vector2 pos;
	
    private bool _locked;
    private bool _attack;
    private GameObject _target;
    private List<Star> _selectedStars = new List<Star>();
    private float _distance;
    public Rigidbody2D player;
    private Transform _tr;

    private Vector3 _direction;
    

    void Awake()
    {
        _tr = GetComponent<Transform>();
    }
    
    void Update()
    {
        // Understand if the player is moving the mouse. If it is so, move the viewfinder alongside it.
        if (Math.Abs(Input.GetAxis("StarViewfinderVerticalMouse")) > 0.001f)
        {
            _tr.position = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        // Get the controls of the viewfinder done with the joystick.
        horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
        verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
        _direction = new Vector3(horizontalMove, verticalMove);
        
        if (Input.GetButtonDown("LockStarAttack"))
        {
            _locked = true;
        }
        
        if (Input.GetButtonDown("Attack"))
        {
            _attack = true;
        }

        if (_locked)
        {
            TargetStar();
        }

        if (_attack)
        {
            Attack();
        }
        

    }
    
    private void TargetStar()
    {
        // Get viewfinder position as a Vector2
        Vector2 viewFinderPosition = _tr.position;

        // Cast a ray from player to viewfinder position
        Vector2 relativeViewFinderPosition = viewFinderPosition - player.position;
        RaycastHit2D hit = Physics2D.Raycast(player.position, relativeViewFinderPosition, Mathf.Infinity, starLayerMask);

        // If the ray hits a star, put it in the selected stars for attack.
        // Else, empties the list (consider it as a discard attack).
        if (hit.rigidbody != null)
        {
            Star star = (Star) hit.rigidbody.gameObject.GetComponent(typeof(Star));
            if (star != null)
            {
                star.Select();
                _selectedStars.Add(star);
            }
        }
        else
        {
            foreach (Star star in _selectedStars)
            {
                star.Deselect();
            }
        }

        _locked = false;
    }

    private void Attack()
    {
        // Get viewfinder position as a Vector2
        Vector2 viewFinderPosition = _tr.position;

        // Cast a ray from player to viewfinder position
        Vector2 relativeViewFinderPosition = viewFinderPosition - player.position;
        RaycastHit2D hit = Physics2D.Raycast(player.position, relativeViewFinderPosition, Mathf.Infinity, enemyLayerMask);

        // If the ray hits an enemy, perform the attack.
        // Else, empties the list (consider it as a discard attack).
        if (hit.rigidbody != null)
        {
            Enemy enemy = (Enemy) hit.rigidbody.gameObject.GetComponent(typeof(Enemy));
            if (enemy != null)
            {
                enemy.TakeDamage(_selectedStars.Count*10);
                foreach (Star star in _selectedStars)
                {
                    star.Deselect();
                }
            }
        }
        else
        {
            foreach (Star star in _selectedStars)
            {
                star.Deselect();
            }
        }

        _attack = false;

    }

    private void FixedUpdate()
    {
        _tr.localPosition = _tr.localPosition + _direction * moveSpeed * Time.deltaTime;
    }
}

