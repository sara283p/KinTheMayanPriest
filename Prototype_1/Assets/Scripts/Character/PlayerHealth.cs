using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float max_health = 100f;
    public float cur_health = 0f;
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
        Spawn();

        rend = GetComponent<Renderer>();
        c = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (!alive)
        {
            return;
        }

        if (cur_health - amount <= 0)
        {
            cur_health = 0;
            alive = false;
            Spawn();
        }
        else
        {
            cur_health -= amount;
            StartCoroutine("GetInvulnerable");
        }
    }

    public void Die()
    {
        TakeDamage(max_health);
    }

    private void Spawn()
    {
        transform.position = reSpawnPoint.transform.position;
        cur_health = max_health;
        alive = true;
        
    }
    
    IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        c.a = 0.5f;
        rend.material.color = c;
        yield return new WaitForSeconds(invulnerabilityTime);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        c.a = 1f;
        rend.material.color = c;
    }
}
