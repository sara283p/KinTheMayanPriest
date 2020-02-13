using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianaSeamlessPiece : MonoBehaviour
{
    private CapsuleCollider cc;
    private CapsuleCollider2D cc2;
    private SpriteRenderer _renderer;

    private void OnEnable()
    {
        cc2 = GetComponent<CapsuleCollider2D>();
        cc = GetComponent<CapsuleCollider>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void DestroyPiece()
    {
        StartCoroutine(AnimationRoutine());
    }

    IEnumerator AnimationRoutine()
    {
    float time = 0.5f;
    var solidColor = _renderer.color;
    var transparentColor = solidColor;

    if (cc2) cc2.enabled = false;
    if (cc) cc.enabled = false;

    float dec = 0.01f;
        
        for (int i = 0; i < 100; i++)
    {
        transparentColor.a -= dec;
        _renderer.material.color = transparentColor;
        yield return new WaitForSeconds(0.001f);
    }
        
    Destroy(gameObject);
    }
}
