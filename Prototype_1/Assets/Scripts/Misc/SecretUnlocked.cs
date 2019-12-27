using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class SecretUnlocked : MonoBehaviour
{
    private List<Collider2D> _disabledColliders;
    private CinemachineConfiner _confiner;

    private void Awake()
    {
        _disabledColliders = GetComponents<Collider2D>()
            .Where(coll => !coll.enabled)
            .ToList();
        _confiner = GameObject.FindObjectOfType<CinemachineConfiner>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("SecretUnlocked", UnlockSecretArea);
    }

    private void OnDisable()
    {
        EventManager.StopListening("SecretUnlocked", UnlockSecretArea);
    }

    private void UnlockSecretArea()
    {
        _disabledColliders
            .ForEach(coll =>
            {
                coll.enabled = true;
                coll.usedByComposite = true;
            });
        _confiner.InvalidatePathCache();
        StartCoroutine(SecretAreaDamping());
    }

    private IEnumerator SecretAreaDamping()
    {
        _confiner.m_Damping = 1;
        yield return new WaitForSeconds(3);
        _confiner.m_Damping = 0;
    }
}
