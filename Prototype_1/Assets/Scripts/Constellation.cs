using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    private float _extent;
    private BoxCollider2D _collider;
    private Transform _tr;

    public float GetLeftBound()
    {
        return _tr.position.x - _extent;
    }

    public float GetRightBound()
    {
        return _tr.position.x + _extent;
    }

    public float GetExtent()
    {
        return _extent;
    }

    
    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _extent = 19.51202f / 2f;
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
