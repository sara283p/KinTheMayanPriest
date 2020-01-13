using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    private float _horizontalExtent;
    private float _verticalExtent;
    private Transform _tr;
    private float _width;
    private float _height;

    public float GetWidth()
    {
        return _width;
    }

    public float GetHeight()
    {
        return _height;
    }
    
    public float GetLeftBound()
    {
        return _tr.position.x - _horizontalExtent;
    }

    public float GetRightBound()
    {
        return _tr.position.x + _horizontalExtent;
    }

    public float GetHorizontalExtent()
    {
        return _horizontalExtent;
    }

    public float GetVerticalExtent()
    {
        return _verticalExtent;
    }

    public float GetBottomBound()
    {
        return _tr.position.y - _verticalExtent;
    }

    
    private void Awake()
    {
        _tr = GetComponent<Transform>();
        // Value of the extent computed as BoxCollider2D's width divided by 2
        Vector2 size = GetComponent<BoxCollider2D>().size;
        _width = size.x;
        _height = size.y;
        _horizontalExtent = _width / 2f;
        _verticalExtent = _height / 2f;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ConstellationDespawner"))
        {
            EventManager.TriggerEvent("DespawnConstellation");
        }
    }
}
