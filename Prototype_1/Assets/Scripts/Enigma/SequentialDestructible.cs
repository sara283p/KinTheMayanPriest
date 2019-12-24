using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialDestructible : MonoBehaviour, IDamageable
{
    [Min(0)] public int sequenceOrder;
    private bool _isDestroyed;
    private Enigma _enigma;

    private void Awake()
    {
        _enigma = transform.parent.GetComponent<Enigma>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        bool correctSequence = _enigma.CheckSequence(sequenceOrder);
        if (correctSequence)
        {
            _isDestroyed = true;
            gameObject.SetActive(false);
        }
        else
        {
            _enigma.ReinitializeEnigma();
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsDestroyed => _isDestroyed;
}
