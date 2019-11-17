using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float max_health = 100f;
    public float cur_health = 0f;
    public bool alive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        cur_health = max_health;
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
        }
    }
}
