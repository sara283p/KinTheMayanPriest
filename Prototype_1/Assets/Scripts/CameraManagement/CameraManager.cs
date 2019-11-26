using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CinemachineConfiner _confiner;

    private void Awake()
    {
        _confiner = GetComponent<CinemachineConfiner>();
    }

    public static void ChangeCamera(GameObject toDisable, GameObject toEnable)
    {
        toDisable.SetActive(false);
        CinemachineVirtualCamera asdf = toEnable.GetComponentInChildren<CinemachineVirtualCamera>(true);
        asdf.gameObject.SetActive(true);
    }
}
