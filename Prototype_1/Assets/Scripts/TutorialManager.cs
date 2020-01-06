using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject channelLight;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private GameObject releaseEnergy;
    private SpriteRenderer rendAimTarget;
    private SpriteRenderer rendReleaseEnergy;

    private void Start()
    {
        rendAimTarget = aimTarget.GetComponent<SpriteRenderer>();
        Color cAimTarget = rendAimTarget.material.color;
        cAimTarget.a = 0f;
        rendAimTarget.material.color = cAimTarget;

        rendReleaseEnergy = releaseEnergy.GetComponent<SpriteRenderer>();
        Color cReleaseEnergy = rendReleaseEnergy.material.color;
        cReleaseEnergy.a = 0f;
        rendReleaseEnergy.material.color = cReleaseEnergy;
    }

    private void OnEnable()
    {
        EventManager.StartListening("StarSelected", ShowAimTarget);
    }

    private void ShowAimTarget()
    {
        StartCoroutine("FadeInAimTarget");
        EventManager.StartListening("TargetAcquired", ShowReleaseEnergy);
        EventManager.StopListening("StarSelected", ShowAimTarget);
    }

    private void ShowReleaseEnergy()
    {
        StartCoroutine("FadeInReleaseEnergy");
        EventManager.StopListening("TargetAcquired", ShowReleaseEnergy);
    }

    IEnumerator FadeInAimTarget()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = rendAimTarget.material.color;
            c.a = f;
            rendAimTarget.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    IEnumerator FadeInReleaseEnergy()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = rendReleaseEnergy.material.color;
            c.a = f;
            rendReleaseEnergy.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
