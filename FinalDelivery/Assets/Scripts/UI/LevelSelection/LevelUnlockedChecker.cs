using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockedChecker : MonoBehaviour
{
    public String levelName;
    public Sprite lockedLevelIcon;

    private GameObject _dotPath;

    private void Awake()
    {
        _dotPath = GetComponentInChildren<DotPath>().gameObject;
        if (!GameManager.Instance.IsLevelUnlocked(levelName))
        {
            GetComponent<Image>().sprite = lockedLevelIcon;
            GetComponent<Button>().enabled = false;
            _dotPath.SetActive(false);
        }
    }
}
