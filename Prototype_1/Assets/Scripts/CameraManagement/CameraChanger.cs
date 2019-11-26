using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    private CinemachineBrain _camera;

    private void Awake()
    {
        _camera = GetComponent<Transform>().parent.parent.GetComponentInChildren<CinemachineBrain>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Confiner"))
        {
            print("Camera changed confiner");
            CameraManager.ChangeCamera(_camera.ActiveVirtualCamera.VirtualCameraGameObject, other.gameObject);
        }
    }
}
