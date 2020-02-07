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

    [Header("Camera Offsets")]
    [Tooltip("Offset to be added to ScreenX property when entering the trigger from left or top side")]
    [Range(-1, 1)]
    public float leftOrTopOffsetX;
    
    [Tooltip("Offset to be added to ScreenY property when entering the trigger from left or top side")]
    [Range(-1, 1)]
    public float leftOrTopOffsetY;
    
    [Tooltip("Offset to be added to ScreenX property when entering the trigger from right or bottom side")]
    [Range(-1, 1)]
    public float rightOrBottomOffsetX;
    
    [Tooltip("Offset to be added to ScreenY property when entering the trigger from right or bottom side")]
    [Range(-1, 1)]
    public float rightOrBottomOffsetY;
    
    [Header("Trigger settings")]
    public bool isHorizontal;
    public bool oneWay;

    private CinemachineVirtualCamera _virtualCamera;
    private bool _triggerActivated;
    private float _targetSize;
    private Vector2 _unusedVector;
    private Vector2 _enteringSide;
    private bool _alreadyTriggered;
    private bool _offsetsAlreadyApplied;

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

            Vector2 targetVector = new Vector2(_targetSize, 0);
            _virtualCamera.m_Lens.OrthographicSize = Vector2.SmoothDamp(new Vector2(orthographicSize, 0), targetVector, ref _unusedVector, 0.5f).x;
            
            if (targetReached)
            {
                _triggerActivated = false;
                if (oneWay)
                {
                    enabled = false;
                }
            }
        }
    }
    
    private void ApplyOffsets()
    {
        if (_offsetsAlreadyApplied)
            return;
        _offsetsAlreadyApplied = true;
        float offsetX;
        float offsetY;
        if (_enteringSide == Vector2.left || _enteringSide == Vector2.up)
        {
            offsetX = leftOrTopOffsetX;
            offsetY = leftOrTopOffsetY;
        }
        else
        {
            offsetX = rightOrBottomOffsetX;
            offsetY = rightOrBottomOffsetY;
        }
        CinemachineFramingTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        float sum = transposer.m_ScreenX + offsetX;
        if(sum > 0 && sum < 1)
            transposer.m_ScreenX += offsetX;
        sum = transposer.m_ScreenY + offsetY;
        if (sum > 0 && sum < 1)
        {
            transposer.m_ScreenY += offsetY;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (oneWay && _alreadyTriggered)
            return;
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _triggerActivated = true;
            _alreadyTriggered = true;
            if (!isHorizontal)
            {
                if (other.transform.position.x > transform.position.x)
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
                if (other.transform.position.y < transform.position.y)
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
            ApplyOffsets();
        }
    }
}
