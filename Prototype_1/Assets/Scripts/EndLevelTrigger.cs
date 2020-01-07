using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private List<Animator> _godAnimators;
    private bool _alreadyTriggered;
    
    private static readonly int Activated = Animator.StringToHash("Activated");
    
    private void Awake()
    {
        _godAnimators = GetComponentsInChildren<Animator>().ToList();
    }
    
     private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.isTrigger && !_alreadyTriggered)
            {
                StartCoroutine(AnimationEnabler());
                _alreadyTriggered = true;
            }
        }
     
     private IEnumerator AnimationEnabler()
         {
             _godAnimators
                 .ForEach(contr => contr.SetBool(Activated, true));
             yield return new WaitForSeconds(1);
             _godAnimators
                 .ForEach(contr => contr.SetBool(Activated, false));
         }
}
