using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "Kin/ConstellationSet")]
public class ConstellationSet : RuntimeSet<Constellation>
{
    private Random _rand = new Random();
    
    // This list contains all the indexes of the rendered constellations
    private List<int> _indexes = new List<int>();
    
    public Constellation GetRandomConstellation()
    {
        int index;
        if (_indexes.Count == list.Count)
            return null;
        do
        {
            index = _rand.Next(0, list.Count);
        } while (_indexes.Contains(index));
        _indexes.Add(index);
        return Get(index);
    }
}
