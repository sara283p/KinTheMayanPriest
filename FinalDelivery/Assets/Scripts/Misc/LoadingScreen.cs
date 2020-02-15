using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingScreen : MonoBehaviour
{
    private Slider _bar;
    private AsyncOperation _loadingOperation;
    private VideoPlayer _videoPlayer;

    private void Awake()
    {
        _bar = GetComponentInChildren<Slider>(true);
    }

    public void SceneLoading(AsyncOperation loadingOperation)
    {
        gameObject.SetActive(true);
        _bar.value = 0;
        _loadingOperation = loadingOperation;
        _videoPlayer.Play();
        StartCoroutine(SceneLoading());
    }

    public void SetPlayer(VideoPlayer videoPlayer)
    {
        _videoPlayer = videoPlayer;
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
        _videoPlayer.Stop();
        gameObject.SetActive(false);
        _loadingOperation = null;
        EventManager.TriggerEvent("LoadingCompleted");
    }
}
