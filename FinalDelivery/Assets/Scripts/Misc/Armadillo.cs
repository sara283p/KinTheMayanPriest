using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Armadillo : MonoBehaviour
{
    private bool _activated;
    private Animator _animator;
    private Camera _camera;
    private static readonly int Activated = Animator.StringToHash("Activated");

    public float speed;

    private void Awake()
    {
        if (GameManager.Instance.isCollectibleTaken)
        {
            gameObject.SetActive(false);
        }
        _camera = FindObjectOfType<Camera>();
        _animator = GetComponent<Animator>();
    }

    private void Activate()
    {
        _activated = true;
        _animator.SetBool(Activated, true);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            Activate();
            AudioManager.instance.Play("ArmadilloSound");
            GameManager.Instance.TakeCollectible();
        }
    }
}
