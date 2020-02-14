using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlockedChecker : MonoBehaviour
{
    public String levelName;

    private void Awake()
    {
        if (!GameManager.Instance.IsLevelUnlocked(levelName))
        {
            gameObject.SetActive(false);
        }
    }
}
