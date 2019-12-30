using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enigma : MonoBehaviour
{
    private List<SequentialDestructible> _enigmaSet = new List<SequentialDestructible>();

    private void Awake()
    {
        foreach (SequentialDestructible obj in GetComponentsInChildren<SequentialDestructible>())
        {
            _enigmaSet.Add(obj);
        }
    }

    public bool CheckSequence(int destroyedObjectOrder)
    {
        var toReturn = _enigmaSet
            .Where(obj => obj.sequenceOrder < destroyedObjectOrder)
            .Aggregate(true, (init, obj) => !obj.isActiveAndEnabled && init);

        if (toReturn && (_enigmaSet.Count() - _enigmaSet.Count(obj => !obj.isActiveAndEnabled)) == 0)
            StartCoroutine(_EndEnigma());
            
        return toReturn;

    }

    public void ReinitializeEnigma()
    {
        StartCoroutine(_ReinitializeEnigma());
    }
    
    private IEnumerator _ReinitializeEnigma()
    {
        yield return new WaitForSeconds(0.01f);
        _enigmaSet.ForEach(obj => obj.gameObject.SetActive(true));
    }
    
    private IEnumerator _EndEnigma()
    {
        yield return new WaitForSeconds(0.01f);
        print("Enigma solved!");
        gameObject.SetActive(false);
    }
    
}
