using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enigma : MonoBehaviour
{
    private SequenceEnigmaSet _enigmaSet;

    private void Awake()
    {
        _enigmaSet = ScriptableObject.CreateInstance<SequenceEnigmaSet>();

        foreach (SequentialDestructible obj in GetComponentsInChildren<SequentialDestructible>())
        {
            _enigmaSet.Add(obj);
        }
    }

    public bool CheckSequence(int destroyedObjectOrder)
    {
        return _enigmaSet.CheckSequence(destroyedObjectOrder);
    }

    public void ReinitializeEnigma()
    {
        _enigmaSet.ReinitializeEnigma();
    }
}
