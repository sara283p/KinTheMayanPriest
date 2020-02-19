using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private EnemySet _enemies;
    private int _killedEnemies;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _enemies = ScriptableObject.CreateInstance<EnemySet>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();

        foreach (Enemy enemy in GetComponentsInChildren<Enemy>(true))
        {
            _enemies.Add(enemy);
        }

        if (GetComponentInParent<LianaSeamless>())
        {
            return;
        }

        GameManager.Instance.RegisterForRespawn(gameObject);
    }

    private void OnEnable()
    {
        // EventManager.StartListening("EnemyKilled", EnemyKilled);
        EventManager.StartListening(string.Concat("DestroyBarrier", GetInstanceID()), EnemyKilled);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening(string.Concat("DestroyBarrier", GetInstanceID()), EnemyKilled);
    }
    
    private void EnemyKilled()
    {
        _killedEnemies++;
        if (_killedEnemies == _enemies.list.Count)
        {
            // enabled = false;
            StartCoroutine(DestroyAnimation());
        }
    }

    IEnumerator DestroyAnimation()
    {
        float time = 0.5f;
        var solidColor = _renderer.color;
        var transparentColor = solidColor;

        _collider.enabled = false;

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
