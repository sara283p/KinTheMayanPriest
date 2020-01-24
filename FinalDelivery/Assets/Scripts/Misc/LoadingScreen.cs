using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private Slider _bar;
    private float _maxValue;
    private AsyncOperation _loadingOperation;

    private void Awake()
    {
        _bar = GetComponentInChildren<Slider>(true);
        _maxValue = 0.9f;
    }

    public void SceneLoading(AsyncOperation loadingOperation)
    {
        gameObject.SetActive(true);
        _bar.value = 0;
        _loadingOperation = loadingOperation;
        StartCoroutine(SceneLoading());
    }
    
    private IEnumerator SceneLoading()
    {
        while (!_loadingOperation.isDone)
        {
            float progress = _loadingOperation.progress;

            _bar.value = progress / _maxValue;
            yield return null;
        }

        _bar.value = _loadingOperation.progress / _maxValue;
        gameObject.SetActive(false);
        _loadingOperation = null;
        EventManager.TriggerEvent("LoadingCompleted");
    }
}
