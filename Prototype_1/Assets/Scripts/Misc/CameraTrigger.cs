using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public float orthographicSizeBeforeTrigger;
    public float orthographicSizeAfterTrigger;

    private CinemachineVirtualCamera _virtualCamera;
    private bool _triggerActivated;
    private float _targetSize;
    private Vector2 _unusedVector;
    private Vector2 _enteringSide;

    private void Awake()
    {
        _virtualCamera = FindObjectsOfType<CinemachineVirtualCamera>().First(cam => cam.gameObject.activeInHierarchy);
        _unusedVector = Vector2.zero;
    }

    private void Update()
    {
        if (_triggerActivated)
        {
            float orthographicSize = _virtualCamera.m_Lens.OrthographicSize;
            bool enlargingTrigger = orthographicSizeBeforeTrigger <= orthographicSizeAfterTrigger;
            bool targetReached;
            
            if (_enteringSide == Vector2.left)
            {
                targetReached = enlargingTrigger ? orthographicSize >= _targetSize - 0.01f : orthographicSize <= _targetSize + 0.01f;
            }
            else
            {
                targetReached = enlargingTrigger ? orthographicSize <= _targetSize + 0.01f : orthographicSize >= _targetSize - 0.01f;
            }
            
            if (targetReached)
            {
                _triggerActivated = false;
            }
            
            Vector2 targetVector = new Vector2(_targetSize, 0);
            _virtualCamera.m_Lens.OrthographicSize = Vector2.SmoothDamp(new Vector2(orthographicSize, 0), targetVector, ref _unusedVector, 0.5f).x;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _triggerActivated = true;
            if (other.transform.position.x < transform.position.x)
            {
                _enteringSide = Vector2.left;
                _targetSize = orthographicSizeAfterTrigger;
            }
            else
            {
                _enteringSide = Vector2.right;
                _targetSize = orthographicSizeBeforeTrigger;
            }
        }
    }
}
