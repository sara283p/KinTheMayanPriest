using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public String[] scenes;
    public float characterMaxHealth;
    public float obstacleMaxHealth;
    public float enemyPerStarDamage;
    public float starCooldownTime;
    public float maxHangDistance;
    public float maxStarSelectDistance;
    public float minHangDistance;
    public int linkableStars;
    [Range(0, 1)] public float analogDeadZone;
    public float lavaDamage;
    [Range(0, 1)] public float waterSpeedModifier;
    [Range(0, 1)] public float waterGravityModifier;
    
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            EventManager.TriggerEvent("LinkableStarsIncreased");
        }
    }

    public void IncreaseLinkableStars()
    {
        linkableStars++;
        EventManager.TriggerEvent("LinkableStarsIncreased");
        // TODO: save as persistent the new value of maximum linkable stars
    }

    public float GetEnemyHealthFromHits(int hitsToDeath)
    {
        return hitsToDeath * enemyPerStarDamage;
    }
}
