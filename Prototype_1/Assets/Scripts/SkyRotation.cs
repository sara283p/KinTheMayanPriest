using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotation : MonoBehaviour
{
    public GameObject constellationPrefab;

    private ConstellationSet _constellations;
    private Vector2 _position;
    private float _height;
    private float _margin;
    private float _minDistance;
    private float _rightMargin;


    private void GenerateConstellations()
    {
        // For now, it simply takes a prefab and instantiate new objects of that.
        // We may think about writing a constellation generator
        // Of course, we must also add gods' constellations to the set, when needed
        for (int i = 0; i < 5; i++)
        {
            GameObject newObject = Instantiate(constellationPrefab);
            newObject.SetActive(false);
            _constellations.Add(newObject.GetComponent<Constellation>());
        }
    }
    
    private void Awake()
    {
        float newPos;

        _constellations = ScriptableObject.CreateInstance<ConstellationSet>();
        _height = 5f;
        _margin = 17f;
        _minDistance = 4f;
        _rightMargin = 60f;
        Camera mainCamera = Camera.main;
        Vector2 screenBottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector2 screenTopRight = new Vector3(_rightMargin, 0);
        newPos = screenBottomLeft.x + _margin;
        GenerateConstellations();

        Constellation constellation;
        do
        {
            _position = new Vector2(newPos, _height);
            constellation = _constellations.GetRandomConstellation();
            GameObject toRender = constellation.gameObject;
            toRender.GetComponent<Transform>().position = _position;
            toRender.SetActive(true);
            newPos = constellation.GetRightBound() + constellation.GetExtent() + _minDistance;
        } while (newPos < screenTopRight.x);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
