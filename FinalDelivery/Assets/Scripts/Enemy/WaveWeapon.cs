using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveWeapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float firingInterval = 2f;

    public Vector3 spawnPoint;
    public Quaternion bulletAngle;

    private Quaternion firstRotation;
    private Quaternion secondRotation;
    private Quaternion thirdRotation;
    private Quaternion fourthRotation;
    
    void Start()
    {
        spawnPoint = firePoint.position;
        bulletAngle = firePoint.rotation;
        firstRotation = Quaternion.Euler(0, 0, -90);
        secondRotation = Quaternion.Euler(0, 0 , -100);
        thirdRotation = Quaternion.Euler(0, 0, -110);
        fourthRotation = Quaternion.Euler(0, 0 , -120);
        
        StartCoroutine("Shoot");
    }

    void Update()
    {
        spawnPoint = firePoint.position;
        bulletAngle = firePoint.rotation;
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            Instantiate(bulletPrefab, spawnPoint, firstRotation);
            Instantiate(bulletPrefab, spawnPoint, secondRotation);
            Instantiate(bulletPrefab, spawnPoint, thirdRotation);
            Instantiate(bulletPrefab, spawnPoint, fourthRotation);
            yield return new WaitForSeconds(firingInterval);
        }
    }
}
