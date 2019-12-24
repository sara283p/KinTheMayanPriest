using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SequenceEnigmaSet :  RuntimeSet<SequentialDestructible>
{
    public bool CheckSequence(int destroyedObjectOrder)
    {
        return list
            .Where(obj => obj.sequenceOrder < destroyedObjectOrder)
            .Aggregate(true, (init, obj) => obj.IsDestroyed && init);
        
    }

    public void ReinitializeEnigma()
    {
        list.ForEach(obj => obj.gameObject.SetActive(true));
    }
}
