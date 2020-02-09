using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRemoveHolder : MonoBehaviour
{

    [SerializeField] private GameObject enemyHolder;

    private void OnEnable()
    {
        EventManager.StartListening("PlayerRespawn", Reinit);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", Reinit);
    }

    private void Reinit()
    {
        enemyHolder.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("MovingPlatform"))
        {
            enemyHolder.SetActive(false);
        }
    }
}
