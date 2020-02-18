using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRedSphere : MonoBehaviour
{
    public Sprite firstFrame;

    private Vector3 _initialPosition;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        EventManager.StartListening("PlayerRespawn", Init);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("PlayerRespawn", Init);
    }

    private void Init()
    {
        transform.localPosition = _initialPosition;
        _spriteRenderer.sprite = firstFrame;
    }
}
