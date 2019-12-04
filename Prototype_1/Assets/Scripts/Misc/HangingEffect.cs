using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingEffect : MonoBehaviour
{
    private Transform _star;

    public Transform kin;

    public LineRenderer lineRenderer;

    public void StartEffect(Transform target)
    {
        _star = target;
        UpdateLineRendererPosition();
        lineRenderer.enabled = true;
    }

    public void StopEffect()
    {
        lineRenderer.enabled = false;
    }

    private void UpdateLineRendererPosition()
    {
        lineRenderer.SetPosition(0, kin.position);
        lineRenderer.SetPosition(1, _star.position);
    }

    // Update is called once per frame
    // It's called only if this is enabled. No need for further checks.
    void Update()
    {
        if (lineRenderer.enabled)
        {
            UpdateLineRendererPosition();
        }
    }

}
