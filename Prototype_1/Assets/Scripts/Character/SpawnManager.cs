using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float reSpawnTime = 2f;
    private PlayerHealth _playerHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerHealth = FindObjectOfType<PlayerHealth>();
        EventManager.TriggerEvent("PlayerDeath");
    }

    //called when the player dies ("PlayerDeath")
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerCoroutine());
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        _playerHealth.gameObject.SetActive(false);
        //wait for a respawn time interval before actually respawning
        yield return new WaitForSeconds(reSpawnTime);
        _playerHealth.Spawn();
        //to make the player vulnerable again, in case he died while he was invulnerable
        _playerHealth.GetVulnerable();
        _playerHealth.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDeath", RespawnPlayer);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerDeath", RespawnPlayer);
    }
}
