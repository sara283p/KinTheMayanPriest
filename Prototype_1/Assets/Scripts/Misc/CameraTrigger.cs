using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Header("Camera Orthographic Size settings")]
    [Tooltip("With vertical trigger, \"Orthographic Size Before Trigger\" " +
              "is the orthographic size value the camera must have to the left " +
              "of the trigger; with horizontal trigger, it is the orthographic" +
              "size value the camera must have above the trigger.")]
    public float orthographicSizeBeforeTrigger;
    [Tooltip("With vertical trigger, \"Orthographic Size After Trigger\" " +
             "is the orthographic size value the camera must have to the right " +
             "of the trigger; with horizontal trigger, it is the orthographic" +
             "size value the camera must have below the trigger.")]
    public float orthographicSizeAfterTrigger;
    
    [Header("Trigger settings")]
    public bool isHorizontal;
    public bool oneWay;

    private CinemachineVirtualCamera _virtualCamera;
    private bool _triggerActivated;
    private float _targetSize;
    private Vector2 _unusedVector;
    private Vector2 _enteringSide;
    private bool _alreadyTriggered;

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

            if (_enteringSide == Vector2.left || _enteringSide == Vector2.up)
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
        if (oneWay && _alreadyTriggered)
            return;
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _triggerActivated = true;
            _alreadyTriggered = true;
            if (!isHorizontal)
            {
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
            else
            {
                if (other.transform.position.y > transform.position.y)
                {
                    _enteringSide = Vector2.up;
                    _targetSize = orthographicSizeAfterTrigger;
                }
                else
                {
                    _enteringSide = Vector2.down;
                    _targetSize = orthographicSizeBeforeTrigger;
                }
            }
        }
    }
}
