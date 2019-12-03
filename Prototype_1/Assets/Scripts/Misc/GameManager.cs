using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public String[] scenes;
    public float enemyMaxHealth;
    public float characterMaxHealth;
    public float obstacleMaxHealth;
    public float enemyPerStarDamage;
    public float obstaclePerStarDamage;
    
    private static GameManager _manager;
    
    
    public static GameManager Instance => _manager;

    private void Awake()
    {
        if (!_manager)
        {
            _manager = this;

            if (!_manager)
            {
                print("There must be an active GameManager");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(scenes[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(scenes[1]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(scenes[2]);
        }
    }
}
