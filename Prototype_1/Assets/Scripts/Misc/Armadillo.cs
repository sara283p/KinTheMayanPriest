using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Armadillo : MonoBehaviour
{
    private bool _activated;
    private Camera _camera;
    
    public float speed;

    private void Awake()
    {
        _camera = GameObject.FindObjectOfType<Camera>();
    }

    public void Activate()
    {
        _activated = true;
    }
    
    void FixedUpdate()
    {
        if (_activated)
        {
            float delta = speed * Time.fixedDeltaTime;
            Transform tr = transform;
            
            Vector2 targetPos = tr.position;
            targetPos.x += delta;
            tr.position = targetPos;

            if (_camera.WorldToScreenPoint(tr.position).x > _camera.pixelWidth + _camera.pixelWidth / 2)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
