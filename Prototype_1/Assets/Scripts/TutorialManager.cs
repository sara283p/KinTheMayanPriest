using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject channelLight;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private GameObject releaseEnergy;

    private void Start()
    {
        channelLight.SetActive(true);
        aimTarget.SetActive(false);
        releaseEnergy.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.StartListening("StarSelected", ShowAimTarget);
    }

    private void ShowAimTarget()
    {
        aimTarget.SetActive(true);
        EventManager.StartListening("TargetAcquired", ShowReleaseEnergy);
        EventManager.StopListening("StarSelected", ShowAimTarget);
    }

    private void ShowReleaseEnergy()
    {
        releaseEnergy.SetActive(true);
        EventManager.StopListening("TargetAcquired", ShowReleaseEnergy);
    }
}
