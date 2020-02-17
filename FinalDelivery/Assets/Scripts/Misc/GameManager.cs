using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Misc;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public String[] scenes;
    public Canvas loadingScreen;
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

    private bool[] _isLevelCompleted;
    private bool[] _isCollectibleTaken;
    private List<GameObject> _registeredForRespawn;
    private List<GameObject> _aliveObjects;
    private bool _respawnAlreadyRegistered;
    private List<bool> _activeFlags;
    private static GameManager _manager;
    private int _currentLevel;
    private bool _isChangingLevel;
    private LoadingScreen _loadingScreen;
    
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
            _loadingScreen = loadingScreen.GetComponent<LoadingScreen>();

            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            RenderTexture texture = new RenderTexture(Screen.width, Screen.height, 24);
            RawImage panelImage = GetComponentInChildren<RawImage>(true);
            videoPlayer.targetTexture = texture;
            panelImage.texture = texture;
            videoPlayer.Prepare();

            
            _loadingScreen.SetPlayer(videoPlayer);

            LoadPersistentData();

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

    private void LoadPersistentData()
    {
        String dataFile = "data.dat";
        bool fileCreated = false;

        if(!File.Exists(Application.persistentDataPath + dataFile))
        {
            fileCreated = true;
            ReinitializePersistentData();
        }

        if(!fileCreated){
            FileStream f = File.Open(Application.persistentDataPath + dataFile, FileMode.Open);
            PersistentData data = (PersistentData) new BinaryFormatter().Deserialize(f);
            _isLevelCompleted = data.completedLevels;
            _isCollectibleTaken = data.collectedCollectibles;
            linkableStars = data.linkableStars;
            f.Close();
        }
    }

    public void ReinitializePersistentData()
    {
        _isLevelCompleted = scenes.Select(x => false).ToArray();
        _isCollectibleTaken = scenes.Select(x => false).ToArray();
        linkableStars = 1;
        FileStream f = File.Create(Application.persistentDataPath + "data.dat");
        new BinaryFormatter().Serialize(f, new PersistentData(_isLevelCompleted, _isCollectibleTaken, linkableStars));
        f.Close();
    }

    private void SavePersistentData()
    {
        FileStream f = File.Open(Application.persistentDataPath + "data.dat", FileMode.Open);
        new BinaryFormatter().Serialize(f, new PersistentData(_isLevelCompleted, _isCollectibleTaken, linkableStars));
        f.Close();
    }

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        EventManager.StartListening("PlayerRespawn", Reinit);
        EventManager.StartListening("LevelFinished", ChangeLevel);
        EventManager.StartListening("LoadingCompleted", LevelLoaded);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerRespawn", Reinit);
        EventManager.StopListening("LevelFinished", ChangeLevel);
        EventManager.StopListening("LoadingCompleted", LevelLoaded);
    }

    private void LevelLoaded()
    {
        _isChangingLevel = false;
    }

    private void ReinitRespawnLists()
    {
        _registeredForRespawn.Clear();
        _aliveObjects.Clear();
        _activeFlags.Clear();
        _respawnAlreadyRegistered = false;
    }
    private void ChangeLevel()
    {
        _isChangingLevel = true;
        ReinitRespawnLists();
        if (!_isLevelCompleted[_currentLevel])
        {
            IncreaseLinkableStars();
        }
        _isLevelCompleted[_currentLevel] = true;


        if (_currentLevel == 2)
        {
            _currentLevel = 0;
            SavePersistentData();
            SceneManager.LoadScene(scenes[3]);
        }
        else
        {
            _currentLevel++;
            SavePersistentData();
            _loadingScreen.SceneLoading(SceneManager.LoadSceneAsync(scenes[_currentLevel]));
        }
        
    }

    private void ChangeLevel(int selected)
    {
        _isChangingLevel = true;
        ReinitRespawnLists();

        _currentLevel = selected;
        _loadingScreen.SceneLoading(SceneManager.LoadSceneAsync(scenes[_currentLevel]));
    }

    public void ChangeLevel(String levelName, GameObject activeCanvas)
    {
        int levelIndex = scenes.ToList().IndexOf(levelName);
        if (levelIndex >= 0)
        {
            for (int i = 0; i < levelIndex; i++)
            {
                if (!_isLevelCompleted[i])
                    return;
            }
            activeCanvas.SetActive(false);
            ChangeLevel(levelIndex);
        }
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
        if (!_isChangingLevel)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeLevel(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeLevel(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeLevel(2);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                EventManager.TriggerEvent("LinkableStarsIncreased");
            }
        }
    }

    private void IncreaseLinkableStars()
    {
        linkableStars++;
        EventManager.TriggerEvent("LinkableStarsIncreased");
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

    public void TakeCollectible()
    {
        _isCollectibleTaken[_currentLevel] = true;
    }

    public bool IsNewGame(String levelName)
    {
        int levelIndex = scenes.ToList().IndexOf(levelName);
        return levelIndex == 0 && !_isLevelCompleted[0];
    }

    public bool IsLevelUnlocked(String levelName)
    {
        int levelIndex = scenes.ToList().IndexOf(levelName);
        if (levelIndex == 0)
        {
            return true;
        }

        for (int i = 0; i < levelIndex; i++)
        {
            if (!_isLevelCompleted[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool isCollectibleTaken => _isCollectibleTaken[_currentLevel];
}
