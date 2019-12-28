using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : Health
{

    private float _maxHealth = 100f;
    private float _curHealth = 0f;
    private bool _isVulnerable;
    public bool alive = true;
    
    [SerializeField] private Transform reSpawnPoint;

    private Renderer rend;
    private Color c;

    [SerializeField] private float invulnerabilityTime = 3f;
    [SerializeField] private int playerLayer = 9;
    [SerializeField] private int enemyLayer = 11;
    

    // Start is called before the first frame update
    void Start()
    {
        _maxHealth = GameManager.Instance.characterMaxHealth;
        Spawn();
        rend = GetComponent<Renderer>();
        c = rend.material.color;
        _isVulnerable = true;
    }

    public void TakeDamage(float amount)
    {
        if (!alive || !_isVulnerable)
        {
            return;
        }

        if (_curHealth - amount <= 0)
        {
            _curHealth = 0;
            alive = false;
            EventManager.TriggerEvent("PlayerDeath");
        }
        else
        {
            _curHealth -= amount;
            StartCoroutine("GetInvulnerable");
        }
    }

    //to be called when the player should die no matter how healthy he is (for example when he falls)
    public void Die()
    {
        _isVulnerable = true;
        TakeDamage(_maxHealth);
    }

    public void Spawn()
    {
        //move the player to the respawn point
        transform.position = reSpawnPoint.transform.position;
        _curHealth = _maxHealth;
        alive = true;
        
    }
    
    //to be called when the player gets hit by an enemy, to make him invulnerable for a little interval of time
    IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        _isVulnerable = false;
        //make the player semitransparent
        c.a = 0.5f;
        rend.material.color = c;
        //wait for the invulnerability time before making the player vulnerable again
        yield return new WaitForSeconds(invulnerabilityTime);
        GetVulnerable();
    }

    //make the player vulnerable again. Used also by SpawnManager to make the player vulnerable after respawn,
    //in case he died while he was invulnerable (for example falling)
    public void GetVulnerable()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        _isVulnerable = true;
        //make the player no more semitrasparent
        c.a = 1f;
        rend.material.color = c;
    }

    public override float GetHealth()
    {
        return _curHealth;
    }

    public override float GetMaxHealth()
    {
        return _maxHealth;
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("PlayerFell", Die);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("PlayerFell", Die);
    }
}
