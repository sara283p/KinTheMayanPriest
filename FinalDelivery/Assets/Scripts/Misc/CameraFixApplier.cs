using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class CameraFixApplier : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    
    private GameObject _healthBar;
    private GameObject _powerBar;
    private float _distance;
    private CinemachineVirtualCamera _virtualCamera;
    private Camera _camera;
    private float _initialOrthographicSize;
    private float _initialScreenX;
    private float _initialScreenY;
    private CinemachineFramingTransposer _transposer;

    private void Awake()
    {
        _healthBar = GetComponentInChildren<HealthBar>().gameObject;
        _powerBar = GetComponentInChildren<PowerBar>().gameObject;
        _virtualCamera = FindObjectsOfType<CinemachineVirtualCamera>().First(cam => cam.gameObject.activeInHierarchy);
        _camera = FindObjectOfType<Camera>();
        _initialOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _initialScreenX = _transposer.m_ScreenX;
        _initialScreenY = _transposer.m_ScreenY;
        _distance = ((Vector2) (_healthBar.transform.position - _powerBar.transform.position)).magnitude;
    }

    private void ReinitCamera()
    {
        _virtualCamera.m_Lens.OrthographicSize = _initialOrthographicSize;
        _transposer.m_ScreenX = _initialScreenX;
        _transposer.m_ScreenY = _initialScreenY;
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerRespawn", ReinitCamera);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", ReinitCamera);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 zeroVect = Vector2.zero;
        Vector3 topLeft = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height));
        Vector2 healthBarPos = _healthBar.transform.position;
        topLeft.x += offsetX;
        topLeft.y -= offsetY;
        topLeft.z = 0;

        Vector2.SmoothDamp(healthBarPos, topLeft, ref zeroVect, 0.5f);
        _healthBar.transform.position = topLeft;
        topLeft.y -= _distance;
        topLeft = healthBarPos;
        topLeft.y -= _distance;
        _powerBar.transform.position = topLeft;
        
    }
}
