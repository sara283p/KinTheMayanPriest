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
    private float _actualScale;
    private static float _targetScale;
    [SerializeField] private float _smoothTime;
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
        _targetScale = 1;
    }

    private void ReinitCamera()
    {
        _virtualCamera.m_Lens.OrthographicSize = _initialOrthographicSize;
        _transposer.m_ScreenX = _initialScreenX;
        _transposer.m_ScreenY = _initialScreenY;
        ScaleHUD(1/_actualScale);
        _actualScale = 1;
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
        _healthBar.transform.localScale /= _actualScale;
        _powerBar.transform.localScale /= _actualScale;
        _kinHead.transform.localScale /= _actualScale;
        
        Vector2 zeroVect = Vector2.zero;
        Vector2 actualVect = new Vector2(_actualScale, 0);
        Vector2 targetVect = new Vector2(_targetScale, 0);
        _actualScale = Vector2.SmoothDamp(actualVect, targetVect, ref zeroVect, _smoothTime).x;

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
        
        _healthBar.transform.localScale *= _actualScale;
        _powerBar.transform.localScale *= _actualScale;
        _kinHead.transform.localScale *= _actualScale;
    }

    public static void ScaleHUD(float scaleMultiplier)
    {
        if (Math.Abs(_targetScale - scaleMultiplier) > 0.01)
        {
            _targetScale *= scaleMultiplier;
            
        }
    }
}
