// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// public class SequenceEnigmaSet :  RuntimeSet<SequentialDestructible>
// {
//     public bool CheckSequence(int destroyedObjectOrder)
//     {
//         return list
//             .Where(obj => obj.sequenceOrder < destroyedObjectOrder)
//             // .Aggregate(true, (init, obj) => obj.IsDestroyed && init);
//             .Aggregate(true, (init, obj) => !obj.isActiveAndEnabled && init);
//         
//     }
//
//     public void ReinitializeEnigma()
//     {
//         list.ForEach(obj => obj.gameObject.SetActive(true));
//     }
//
//     public int GetRemaining()
//     {
//         // return (list.Count() - list.Count(obj => obj.IsDestroyed));
//         return (list.Count() - list.Count(obj => !obj.isActiveAndEnabled));
//     }
// }
