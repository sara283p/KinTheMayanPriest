using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enigma : MonoBehaviour
{
    private List<SequentialDestructible> _enigmaSet;
    private List<GameObject> _glyphCopies;
    private bool _started;

    private void Awake()
    {
        _glyphCopies = new List<GameObject>();
        _enigmaSet = new List<SequentialDestructible>();
        
        foreach (SequentialDestructible obj in GetComponentsInChildren<SequentialDestructible>(true))
        {
            _glyphCopies.Add(obj.gameObject);
        }
        
        _glyphCopies.ForEach(obj =>
        {
            obj.SetActive(false);
            GameObject toSpawn = Instantiate(obj, obj.transform.parent);
            toSpawn.SetActive(true);
            _enigmaSet.Add(toSpawn.GetComponent<SequentialDestructible>());
        });

        _started = true;
    }

    public bool CheckSequence(int destroyedObjectOrder)
    {
        if (!_started || GameManager.Instance.IsChangingLevel())
        {
            return false;
        }
        var toReturn = _enigmaSet
            .Where(obj => obj.sequenceOrder < destroyedObjectOrder)
            .Aggregate(true, (init, obj) => (obj == null || obj.isDestroyed) && init);

        if (toReturn && (_enigmaSet.Count() - _enigmaSet.Count(obj => obj == null || obj.isDestroyed)) == 0)
            StartCoroutine(_EndEnigma());
            
        return toReturn;

    }

    public void ReinitializeEnigma()
    {
        if (!_started || GameManager.Instance.IsChangingLevel())
        {
            return;
        }
        StartCoroutine(_ReinitializeEnigma());
    }
    
    private IEnumerator _ReinitializeEnigma()
    {
        yield return new WaitForSeconds(0.01f);

        _started = false;
        
        _enigmaSet.ForEach(destr =>
        {
            if(destr)
                Destroy(destr.gameObject);
        });
        
        _enigmaSet.Clear();

        _glyphCopies
            .ForEach(obj =>
            {
                GameObject toSpawn = Instantiate(obj, obj.transform.parent);
                toSpawn.SetActive(true);
                _enigmaSet.Add(toSpawn.GetComponent<SequentialDestructible>());
            });

        _started = true;
    }
    
    private IEnumerator _EndEnigma()
    {
        yield return new WaitForSeconds(0.01f);
        print("Enigma solved!");
    }
    
}
