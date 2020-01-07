using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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
    [Range(0, 1)] public float attackBonus;
    [Range(0, 1)] public float analogDeadZone;
    public float lavaDamage;
    [Range(0, 1)] public float waterSpeedModifier;
    [Range(0, 1)] public float waterGravityModifier;
    [Range(0, 1)] public float lavaSpeedModifier;
    [Range(0, 1)] public float lavaGravityModifier;

    private List<GameObject> _registeredForRespawn;
    private List<GameObject> _aliveObjects;
    private bool _respawnAlreadyRegistered;
    private List<bool> _activeFlags;
    private static GameManager _manager;
    private int _currentLevel;
    private bool _isChangingLevel;
    
    public static GameManager Instance => _manager;

    private void Awake()
    {
        if (!_manager)
        {
            _manager = this;
            _currentLevel = 0;
            _registeredForRespawn = new List<GameObject>();
            _aliveObjects = new List<GameObject>();
            _activeFlags = new List<bool>();

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

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        EventManager.StartListening("PlayerDeath", Reinit);
        EventManager.StartListening("LevelFinished", ChangeLevel);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerDeath", Reinit);
        EventManager.StopListening("LevelFinished", ChangeLevel);
    }

    private void ChangeLevel()
    {
        _isChangingLevel = true;
        _registeredForRespawn.Clear();
        _aliveObjects.Clear();
        _activeFlags.Clear();
        _respawnAlreadyRegistered = false;
        IncreaseLinkableStars();
        _currentLevel++;
        SceneManager.LoadScene(scenes[_currentLevel]);
        _isChangingLevel = false;
    }

    public bool IsChangingLevel()
    {
        return _isChangingLevel;
    }

    private void Reinit()
    {
        _aliveObjects.ForEach(Destroy);
        _aliveObjects.Clear();
        _respawnAlreadyRegistered = true;
        for (int i = 0; i < _registeredForRespawn.Count; i++)
        {
            GameObject obj = _registeredForRespawn[i];
            GameObject toSpawn = Instantiate(obj, obj.transform.parent);
            _aliveObjects.Add(toSpawn);
            toSpawn.SetActive(_activeFlags[i]);
        }
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            EventManager.TriggerEvent("LinkableStarsIncreased");
        }
    }

    private void IncreaseLinkableStars()
    {
        linkableStars++;
        EventManager.TriggerEvent("LinkableStarsIncreased");
        // TODO: save as persistent the new value of maximum linkable stars
    }

    public float GetEnemyHealthFromHits(float hitsToDeath)
    {
        return hitsToDeath * enemyPerStarDamage;
    }

    public void RegisterForRespawn(GameObject registrar)
    {
        if (_respawnAlreadyRegistered)
        {
            return;
        }
        bool isActive = registrar.activeInHierarchy;
        registrar.SetActive(false);
        _registeredForRespawn.Add(registrar);
        _activeFlags.Add(isActive);
    }
}
