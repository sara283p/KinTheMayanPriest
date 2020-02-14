using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingEffect : MonoBehaviour
{
    private Transform _star;

    public Transform kin;

    public LineRenderer lineRenderer;

    public Sprite normalViewFinder;
    public Sprite hangingViewFinder;

    private GameObject _blueSphere;
    private Animator _blueSphereAnimator;
    private SpriteRenderer _viewfinderSpriteRenderer;
    
    private static readonly int IsHanged = Animator.StringToHash("IsHanged");

    private void Awake()
    {
        _blueSphere = transform.parent.GetComponentInChildren<HangBlueSphere>().gameObject;
        _blueSphereAnimator = _blueSphere.GetComponent<Animator>();
        _blueSphere.SetActive(false);
        _viewfinderSpriteRenderer = transform.parent.parent
            .GetComponentInChildren<MoveStarViewfinder_Joystick>()
            .GetComponent<SpriteRenderer>();
    }

    public void StartEffect(Transform target)
    {
        _star = target;
        UpdateLineRendererPosition();
        lineRenderer.enabled = true;
        _blueSphere.SetActive(true);
        _blueSphereAnimator.SetBool(IsHanged, true);
        _viewfinderSpriteRenderer.sprite = hangingViewFinder;
    }

    public void StopEffect()
    {
        lineRenderer.enabled = false;
        _blueSphereAnimator.SetBool(IsHanged, false);
        _blueSphere.SetActive(false);
        _viewfinderSpriteRenderer.sprite = normalViewFinder;
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
