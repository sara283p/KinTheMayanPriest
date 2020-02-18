using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaSeamlessPiece : MonoBehaviour
{
    private CapsuleCollider cc;
    private CapsuleCollider2D cc2;
    private SpriteRenderer _renderer;
    private float _fadeoutDuration;

    private float _steps;
    private Color _solidColor;
    private Color _transparentColor;

    private float _dec;
    private float _waitTime;

    private void OnEnable()
    {
        cc2 = GetComponent<CapsuleCollider2D>();
        cc = GetComponent<CapsuleCollider>();
        _renderer = GetComponent<SpriteRenderer>();
        _fadeoutDuration = GetComponentInParent<LianaSeamless>().fadeOutDurationInSeconds;
        
        // If there are 60 frame per seconds
        _steps = _fadeoutDuration * 60;
        _solidColor = _renderer.color;
        _transparentColor = _solidColor;
        
        _dec = 1 / _steps;
        _waitTime = _fadeoutDuration / _steps;
    }

    public void DestroyPiece()
    {
        StartCoroutine(AnimationRoutine());
    }

    IEnumerator AnimationRoutine()
    {
        if (cc2) cc2.enabled = false;
        if (cc) cc.enabled = false;

        for (int i = 0; i < _steps; i++)
        {
            _transparentColor.a -= _dec;
            _renderer.material.color = _transparentColor;
            yield return new WaitForSeconds(_waitTime);
        }
            
        Destroy(gameObject);
    }
}
