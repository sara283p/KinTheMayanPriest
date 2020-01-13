using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private List<Animator> _checkPointAnimators;
    private Transform _spawnPoint;
    private bool _alreadyTriggered;

    private static readonly int Activated = Animator.StringToHash("Activated");

    private void Awake()
    {
        _checkPointAnimators = GetComponentsInChildren<Animator>().ToList();
        _spawnPoint = FindObjectOfType<SpawnPoint>().transform;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger && !_alreadyTriggered)
        {
            _spawnPoint.position = (Vector2) transform.position;
            StartCoroutine(AnimationEnabler());
            _alreadyTriggered = true;
        }
    }

    private IEnumerator AnimationEnabler()
    {
        _checkPointAnimators
            .ForEach(contr => contr.SetBool(Activated, true));
        yield return new WaitForSeconds(1);
        _checkPointAnimators
            .ForEach(contr => contr.SetBool(Activated, false));
    }
}
