using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float _startPosX, _startPosY;
    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;
    
    void Awake()
    {
        Vector2 pos = transform.position;
        _startPosX = pos.x;
        _startPosY = pos.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cameraPos = cam.transform.position;
        float distX = (cameraPos.x * parallaxEffectX);
        float distY = (cameraPos.y * parallaxEffectY);
        

        transform.position = new Vector3(_startPosX + distX, _startPosY + distY, transform.position.z);
    }
}
