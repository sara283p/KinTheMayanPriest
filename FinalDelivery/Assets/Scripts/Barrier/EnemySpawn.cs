using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private GameObject _barrier;

    private void Awake()
    {
        _barrier = GetComponentInChildren<Barrier>().gameObject;
        _barrier.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _barrier.SetActive(true);
        }
    }
}
