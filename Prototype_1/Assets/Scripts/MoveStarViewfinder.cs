﻿using System;
using System.Collections;
using System.Collections.Generic;
 using Cinemachine;
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
        _tr.position = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Get the controls of the viewfinder done with the joystick.
        horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
        verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
        _direction = new Vector3(horizontalMove, verticalMove);
        
    }
    
    

    private void FixedUpdate()
    {
        _tr.localPosition = _tr.localPosition + _direction * moveSpeed * Time.deltaTime;
    }
}

