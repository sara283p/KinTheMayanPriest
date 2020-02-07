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
    private Queue<int> _indexes = new Queue<int>();
    
    public Constellation GetRandomConstellation()
    {
        int index;
        if (_indexes.Count == list.Count)
            return null;
        do
        {
            index = _rand.Next(0, list.Count);
        } while (_indexes.Contains(index));
        _indexes.Enqueue(index);
        return Get(index);
    }

    public void DespawnConstellation()
    {
        _indexes.Dequeue();
    }
}
