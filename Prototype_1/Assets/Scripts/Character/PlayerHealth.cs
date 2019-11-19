using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float max_health = 100f;
    public float cur_health = 0f;
    public bool alive = true;

    private Renderer rend;
    private Color c;
    
    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        cur_health = max_health;

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
            gameObject.SetActive(false);
        }
        else
        {
            cur_health -= amount;
            StartCoroutine("GetInvulnerable");
        }
    }

    IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(9, 11, true);
        c.a = 0.5f;
        rend.material.color = c;
        yield return new WaitForSeconds(3f);
        Physics2D.IgnoreLayerCollision(9, 11, false);
        c.a = 1f;
        rend.material.color = c;
    }
}
