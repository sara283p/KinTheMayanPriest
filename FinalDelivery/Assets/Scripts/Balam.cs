using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balam : MonoBehaviour
{
    public Sprite finalFrame;

    private SpriteRenderer _renderer;
    private Animator _animator;
    
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void OnFinishedAnimation()
    {
        _animator.enabled = false;
        _renderer.sprite = finalFrame;
    }
}
