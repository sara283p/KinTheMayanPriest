using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Enigma : MonoBehaviour
{
    public Obstacle obstacle;
    
    private void OnDisable()
    {
        obstacle.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        obstacle.gameObject.SetActive(true);
    }
}
