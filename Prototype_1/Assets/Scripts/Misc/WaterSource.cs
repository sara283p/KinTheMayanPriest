using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class WaterSource : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    public float waterStoppingSeconds;
    
    private List<Animator> _groundLayerAnimators;
    private List<Animator> _backgroundLayerAnimators;
    private Coroutine _runningRoutine;
    private static readonly int IsPlayerTouching = Animator.StringToHash("IsPlayerTouching");

    private void Awake()
    {
        _groundLayerAnimators = GetComponentsInChildren<Animator>()
            .Where(animator => animator.GetComponent<SpriteRenderer>().sortingLayerID == SortingLayer.NameToID("Ground"))
            .ToList();
        _backgroundLayerAnimators = GetComponentsInChildren<Animator>()
            .Where(animator =>
                animator.GetComponent<SpriteRenderer>().sortingLayerID == SortingLayer.NameToID("Background"))
            .ToList();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            if (_runningRoutine != null)
            {
                StopCoroutine(_runningRoutine);
            }
            _groundLayerAnimators.ForEach(animator => animator.speed = 1);
            _backgroundLayerAnimators.ForEach(animator =>
            {

                float rand = UnityEngine.Random.Range(minSpeed, maxSpeed);
                animator.speed = rand;
            });
            
            _backgroundLayerAnimators.Concat(_groundLayerAnimators)
                .ToList()
                .ForEach(animator => animator.SetBool(IsPlayerTouching, true));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.isTrigger)
        {
            _runningRoutine = StartCoroutine(StopWater());
        }
    }

    private IEnumerator StopWater()
    {
        float interExecutionTime = 0.1f;
        int stoppingSteps = Mathf.FloorToInt(waterStoppingSeconds / interExecutionTime);
        float groundDelta = _groundLayerAnimators[0].speed / stoppingSteps;
        List<float> backgroundDeltas = _backgroundLayerAnimators
            .Select(animator => animator.speed)
            .Select(speed => speed / stoppingSteps)
            .ToList();
        
        for(int j = 0; j < stoppingSteps; j++)
        {
            _groundLayerAnimators.ForEach(animator =>
                animator.speed = animator.speed - groundDelta >= 0 ? animator.speed - groundDelta : 0);
            
            for (int i = 0; i < _backgroundLayerAnimators.Count; i++)
            {
                Animator currentAnimator = _backgroundLayerAnimators[i];
                currentAnimator.speed = currentAnimator.speed - backgroundDeltas[i] >= 0 ? currentAnimator.speed - backgroundDeltas[i] : 0;
            }
            
            yield return new WaitForSeconds(interExecutionTime);
        }
        _backgroundLayerAnimators
            .Concat(_groundLayerAnimators)
            .ToList()
            .ForEach(animator =>
            {
                animator.SetBool(IsPlayerTouching, false);
                animator.speed = 1;
            });
    }
}
