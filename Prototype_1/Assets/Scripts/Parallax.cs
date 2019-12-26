using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffectx;
    public float parallaxEffecty;
    
    void Start()
    {
        startpos = transform.position.x;
        startpos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = (cam.transform.position.x * parallaxEffectx);
        float dist2 = (cam.transform.position.y * parallaxEffecty);
        

        transform.position = new Vector3(startpos + dist, startpos + dist2, transform.position.z);
    }
}
