using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private Slider _bar;
    private AsyncOperation _loadingOperation;

    private void Awake()
    {
        _bar = GetComponentInChildren<Slider>(true);
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

            _bar.value = progress;
            yield return null;
        }

        _bar.value = _loadingOperation.progress;
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        _loadingOperation = null;
        EventManager.TriggerEvent("LoadingCompleted");
    }
}
