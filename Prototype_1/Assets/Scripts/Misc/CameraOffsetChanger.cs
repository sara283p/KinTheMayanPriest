using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraOffsetChanger : MonoBehaviour
{
    [Tooltip("Value that will be added to ScreenX property of the active Cinemachine Virtual Camera component")]
    [Range(-1, 1)]
    public float offsetX;
    
    [Range(-1, 1)]
    [Tooltip("Value that will be added to ScreenY property of the active Cinemachine Virtual Camera component")]
    public float offsetY;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = FindObjectsOfType<CinemachineVirtualCamera>().First(cam => cam.gameObject.activeInHierarchy);
    }

    private void ApplyOffsets()
    {
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
        if (other.CompareTag("Player") && other.isTrigger)
        {
            ApplyOffsets();
            enabled = false;
        }
    }
}
