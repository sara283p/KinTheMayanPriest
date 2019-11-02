using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetStar : MonoBehaviour
{
    
    public float thetaScale = 0.01f;
    private float _radius = 0.45f;
    private float _width = 0.15f;
    private int _size;
    private LineRenderer _lineDrawer;
    private float _theta = 0f;

    void Awake()
    {       
        _lineDrawer = GetComponent<LineRenderer>();
        _lineDrawer.widthCurve = AnimationCurve.Constant(0, 0, _width);
        _lineDrawer.sortingOrder = 5;
    }

    void Update ()
    {      
        _theta = 0f;
        _size = (int)((1f / thetaScale) + 1f);
        _lineDrawer.positionCount = _size;
        
        for(int i = 0; i < _size; i++){          
            float x = _radius * Mathf.Cos(_theta);
            float y = _radius * Mathf.Sin(_theta);
            _theta += (2.0f * Mathf.PI * thetaScale);
            _lineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
