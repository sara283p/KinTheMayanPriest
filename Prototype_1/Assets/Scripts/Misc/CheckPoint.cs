using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator _checkPointAnimator;
    private Transform _spawnPoint;

    private static readonly int Activated = Animator.StringToHash("Activated");

    private void Awake()
    {
        _checkPointAnimator = GetComponent<Animator>();
        _spawnPoint = FindObjectOfType<SpawnPoint>().transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _spawnPoint.position = (Vector2) transform.position;
            StartCoroutine(AnimationEnabler());
        }
    }

    private IEnumerator AnimationEnabler()
    {
        _checkPointAnimator.SetBool(Activated, true);
        yield return new WaitForSeconds(1);
        _checkPointAnimator.SetBool(Activated, false);
    }
}
