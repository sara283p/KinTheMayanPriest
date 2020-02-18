using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialDestructible : MonoBehaviour
{
    [Min(0)] public int sequenceOrder;
    // private bool _isDestroyed;
    private Enigma _enigma;
    private bool _isDestroyed;

    private void Awake()
    {
        _enigma = transform.parent.GetComponent<Enigma>();
    }
    
    public void Check()
    {
        _isDestroyed = true;
        bool correctSequence = _enigma.CheckSequence(sequenceOrder);
        if (!correctSequence)
        {
            _enigma.ReinitializeEnigma();
        }
    }

    public bool isDestroyed => _isDestroyed;
}
