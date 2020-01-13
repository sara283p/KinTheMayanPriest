using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleOutline : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Coroutine runningRoutine;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (runningRoutine == null)
        {
            runningRoutine = StartCoroutine(BlinkOutline());
        }
    }

    IEnumerator BlinkOutline()
    {
        Color obstacleColor;
        while (_renderer.color.a > 0)
        {
            obstacleColor = _renderer.color;
            obstacleColor.a -= 0.1f;
            _renderer.color = obstacleColor;
            yield return new WaitForSeconds(0.1f);
        }

        while (_renderer.color.a < 1)
        {
            obstacleColor = _renderer.color;
            obstacleColor.a += 0.1f;
            _renderer.color = obstacleColor;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        runningRoutine = null;
    }
}
