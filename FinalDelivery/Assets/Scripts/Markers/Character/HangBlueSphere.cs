using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangBlueSphere : MonoBehaviour
{
    public Sprite firstFrame;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        EventManager.StartListening("PlayerRespawn", Init);
    }

    private void Init()
    {
        transform.localPosition = _initialPosition;
        _spriteRenderer.sprite = firstFrame;
    }

    private void OnDestroy()
    {
        EventManager.StopListening("PlayerRespawn", Init);
    }
}
