using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerStarter : MonoBehaviour
{
    public GameObject toEnable;
    
    private VideoPlayer _videoPlayer;
    
    private void Awake()
    {
        if(toEnable)
            toEnable.SetActive(false);
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.Prepare();
        StartCoroutine(PlaybackStart());
    }

    private IEnumerator PlaybackStart()
    {
        while (!_videoPlayer.isPrepared)
        {
            yield return null;
        }
        _videoPlayer.Play();
        if(toEnable)
            toEnable.SetActive(true);

    }
}
