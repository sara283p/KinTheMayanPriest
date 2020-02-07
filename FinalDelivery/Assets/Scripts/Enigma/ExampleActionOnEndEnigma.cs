using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleActionOnEndEnigma : MonoBehaviour
{
    public Obstacle stalattite;
    
    private void OnDisable()
    {
        stalattite.gameObject.SetActive(false);
    }
}
