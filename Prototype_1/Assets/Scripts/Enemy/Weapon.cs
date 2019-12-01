using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float firingInterval = 2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Shoot");
    }
    
    


    IEnumerator Shoot()
    {
        while (true)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(firingInterval);
        }
    }

}
