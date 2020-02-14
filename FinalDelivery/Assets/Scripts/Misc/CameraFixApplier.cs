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
    public float headOffsetX;
    public float headOffsetY;
    public float distance;

    private GameObject _healthBar;
    private GameObject _powerBar;
    private GameObject _kinHead;
    private CinemachineVirtualCamera _virtualCamera;
    private Camera _camera;
    private float _initialOrthographicSize;
    private float _initialScreenX;
    private float _initialScreenY;
    private static float _actualScale;
    private Vector3 _originalHealthScale;
    private Vector3 _originalPowerScale;
    private Vector3 _originalHeadScale;
    private CinemachineFramingTransposer _transposer;

    private void Awake()
    {
        _kinHead = GetComponentInChildren<KinHead>().gameObject;
        _healthBar = GetComponentInChildren<HealthBar>().gameObject;
        _powerBar = GetComponentInChildren<PowerBar>().gameObject;
        _virtualCamera = FindObjectsOfType<CinemachineVirtualCamera>().First(cam => cam.gameObject.activeInHierarchy);
        _camera = FindObjectOfType<Camera>();
        _initialOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _initialScreenX = _transposer.m_ScreenX;
        _initialScreenY = _transposer.m_ScreenY;
        _actualScale = 1;
        _originalHealthScale = _healthBar.transform.localScale;
        _originalPowerScale = _powerBar.transform.localScale;
        _originalHeadScale = _kinHead.transform.localScale;
    }

    private void ReinitCamera()
    {
        _virtualCamera.m_Lens.OrthographicSize = _initialOrthographicSize;
        _transposer.m_ScreenX = _initialScreenX;
        _transposer.m_ScreenY = _initialScreenY;
        _actualScale = 1;
        _healthBar.transform.localScale = _originalHealthScale;
        _powerBar.transform.localScale = _originalPowerScale;
        _kinHead.transform.localScale = _originalHeadScale;
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
        Vector3 topLeft = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height));
        Vector2 healthBarPos = _healthBar.transform.position;
        topLeft.x += offsetX * _actualScale;
        topLeft.y -= offsetY * _actualScale;
        topLeft.z = 0;

        _healthBar.transform.position = topLeft;
        topLeft = healthBarPos;
        topLeft.y -= distance * _actualScale;
        _powerBar.transform.position = topLeft;
        Vector2 headPosition = healthBarPos;
        headPosition.x += headOffsetX *_actualScale;
        headPosition.y += headOffsetY * _actualScale;
        _kinHead.transform.position = headPosition;
        
        _healthBar.transform.localScale = _originalHealthScale;
        _powerBar.transform.localScale = _originalPowerScale;
        _kinHead.transform.localScale = _originalHeadScale;
        
        _healthBar.transform.localScale *= _actualScale;
        _powerBar.transform.localScale *= _actualScale;
        _kinHead.transform.localScale *= _actualScale;
    }

    public static void ScaleHUD(float scaleMultiplier)
    {
        _actualScale = scaleMultiplier;
    }
}
