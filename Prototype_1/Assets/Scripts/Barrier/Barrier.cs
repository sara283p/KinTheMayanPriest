using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private EnemySet _enemies;
    private int _killedEnemies;

    private void Awake()
    {
        _enemies = ScriptableObject.CreateInstance<EnemySet>();

        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            _enemies.Add(enemy);
        }
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
            enabled = false;
            Destroy(gameObject);
        }
    }

    // private void Update()
    // {
    //     if (GetComponentsInChildren<Enemy>().Length == 0)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}
