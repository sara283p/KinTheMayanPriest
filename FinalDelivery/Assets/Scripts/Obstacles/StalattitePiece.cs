using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalattitePiece : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Transform _targetPosition;
    private CapsuleCollider2D _collider;
    private Obstacle _parent;
    private bool _obstacleDestroyed;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _targetPosition = transform.parent.GetComponentInChildren<StalactiteTarget>().transform;
        _collider = GetComponent<CapsuleCollider2D>();
        _parent = transform.parent.GetComponent<Obstacle>();
    }

    public void Destroy()
    {
        _obstacleDestroyed = true;
        StartCoroutine(DestroyAnimation());
    }

    private void Update()
    {
        if (_obstacleDestroyed && !_collider.enabled && transform.position.y <= _targetPosition.position.y)
        {
            _collider.enabled = true;
        }
    }

    IEnumerator DestroyAnimation()
    {
        float time = 0.5f;
        var solidColor = _renderer.color;
        var transparentColor = solidColor;
//        transparentColor.a = 0.3f;
        
        yield return new WaitForSeconds(3);

//        for (int i = 0; i < 5; i++)
//        {
//            GetComponent<SpriteRenderer>().material.color = transparentColor;
//            yield return new WaitForSeconds(time);
//            GetComponent<SpriteRenderer>().material.color = solidColor;
//        }
//        GetComponent<SpriteRenderer>().material.color = transparentColor;

        float dec = 0.01f;
        
        for (int i = 0; i < 100; i++)
        {
            transparentColor.a -= dec;
            _renderer.material.color = transparentColor;
            yield return new WaitForSeconds(0.001f);
        }

        _parent.OnChildrenDestroyed();
        Destroy(gameObject);
    }
}
