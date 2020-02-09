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
    private Vector2 _positionSide;
    private bool _alreadyTriggered;
    private Vector2 _previousSide;

    private void Awake()
    {
        _virtualCamera = FindObjectsOfType<CinemachineVirtualCamera>().First(cam => cam.gameObject.activeInHierarchy);
        _unusedVector = Vector2.zero;
    }

    private void DisableTriggerUpdate()
    {
        _triggerActivated = false;
    }

    private void FixedUpdate()
    {
        if (_triggerActivated)
        {
            float orthographicSize = _virtualCamera.m_Lens.OrthographicSize;
            bool enlargingTrigger = orthographicSizeBeforeTrigger <= orthographicSizeAfterTrigger;
            bool targetReached;

            if (_positionSide == Vector2.right || _positionSide == Vector2.down)
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
        float offsetX;
        float offsetY;
        if (_positionSide == Vector2.right || _positionSide == Vector2.down)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneWay && _alreadyTriggered)
            return;
        if (other.CompareTag("Player") && other.isTrigger)
        {
            EventManager.StopListening("CameraTriggerActivated", DisableTriggerUpdate);
            EventManager.TriggerEvent("CameraTriggerActivated");
            _previousSide = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (oneWay && _alreadyTriggered)
            return;
        if (other.CompareTag("Player") && other.isTrigger)
        {
            if (!isHorizontal)
            {
                if (other.transform.position.x > transform.position.x)
                {
                    _positionSide = Vector2.right;
                    _targetSize = orthographicSizeAfterTrigger;
                }
                else
                {
                    _positionSide = Vector2.left;
                    _targetSize = orthographicSizeBeforeTrigger;
                }
            }
            else
            {
                if (other.transform.position.y < transform.position.y)
                {
                    _positionSide = Vector2.down;
                    _targetSize = orthographicSizeAfterTrigger;
                }
                else
                {
                    _positionSide = Vector2.up;
                    _targetSize = orthographicSizeBeforeTrigger;
                }
            }

            bool middleNotPassed = _previousSide == Vector2.zero || _positionSide.Equals(_previousSide);
            _previousSide = _positionSide;
            
            // If this is a oneWay camera trigger and the player didn't pass its center yet, do nothing
            //otherwise, activate the trigger normally
            if (oneWay && middleNotPassed)
                return;

            _triggerActivated = true;
            _alreadyTriggered = true;

            if (!middleNotPassed)
            {
                ApplyOffsets();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            EventManager.StartListening("CameraTriggerActivated", DisableTriggerUpdate);
        }
    }
}
