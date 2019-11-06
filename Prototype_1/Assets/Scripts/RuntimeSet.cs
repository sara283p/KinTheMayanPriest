using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> list = new List<T>();

    public void Add(T item)
    {
        if(!list.Contains(item))
            list.Add(item);
    }

    public void Remove(T item)
    {
        list.Remove(item);
    }

    public T Get(int index)
    {
        return list[index];
    }
}
