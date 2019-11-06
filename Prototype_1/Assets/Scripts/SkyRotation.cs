using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotation : MonoBehaviour
{
    public GameObject constellationPrefab;
    public int constellationCount;

    private ConstellationSet _constellations;
    private Queue<Rigidbody2D> _activeConstellations;
    private float _height;
    private float _margin;
    private float _minDistance;
    private float _rightMargin;
    private bool _rotate;
    public float _speed;
    private float _rightMostConstRBound;


    private void GenerateConstellations()
    {
        // For now, it simply takes a prefab and instantiate new objects of that.
        // We may think about writing a constellation generator
        // Of course, we must also add gods' constellations to the set, when needed
        for (int i = 0; i < constellationCount; i++)
        {
            GameObject newObject = Instantiate(constellationPrefab);
            // The objects are first set to active to call method Awake and initialize its parameters to the
            // desired values, then they are set to false to make them invisible until they are spawned
            // NOTE: without this, the parameters of the constellations not spawned in the Start method
            // will be different, resulting in a bad positioning when they're spawned
            newObject.SetActive(true);
            newObject.SetActive(false);
            _constellations.Add(newObject.GetComponent<Constellation>());
        }
    }

    private void Despawn()
    {
        _constellations.DespawnConstellation();
        _activeConstellations.Dequeue().gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("DespawnConstellation", Despawn);
    }

    private void OnDisable()
    {
        EventManager.StopListening("DespawnConstellation", Despawn);
    }

    private void Awake()
    {
        float newPos;

        _constellations = ScriptableObject.CreateInstance<ConstellationSet>();
        _activeConstellations = new Queue<Rigidbody2D>();
        _height = 5f;
        _margin = 17f;
        _minDistance = 4f;
        _rightMargin = 53f;
        Camera mainCamera = Camera.main;
        Vector2 screenBottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        _rightMostConstRBound = screenBottomLeft.x + _margin;

    }

    private void RotateInput()
    {
        if (Input.GetButtonDown("RotateSky"))
        {
            _rotate = true;
        }
        else if (Input.GetButtonUp("RotateSky"))
        {
            _rotate = false;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateConstellations();
        float newPos = _rightMostConstRBound;
        Constellation constellation;
        do
        {
            Vector2 position = new Vector2(newPos, _height);
            constellation = _constellations.GetRandomConstellation();
            GameObject toRender = constellation.gameObject;
            toRender.GetComponent<Transform>().position = position;
            toRender.SetActive(true);
            _activeConstellations.Enqueue(toRender.GetComponent<Rigidbody2D>());
            newPos = constellation.GetRightBound() + constellation.GetExtent() + _minDistance;

        } while (newPos < _rightMargin);

        _rightMostConstRBound = constellation.GetRightBound();
    }

    // Update is called once per frame
    void Update()
    {
        RotateInput();
        if (_rotate)
        {
            Vector2 delta = new Vector2(- _speed * Time.deltaTime, 0);
            foreach (Rigidbody2D constellation in _activeConstellations)
            {
                constellation.position += delta;
            }
            _rightMostConstRBound += delta.x;

            Vector2 position = new Vector2(_rightMostConstRBound + _minDistance, _height);
            if (position.x < _rightMargin)
            {
                Constellation constellation = _constellations.GetRandomConstellation();
                GameObject toRender = constellation.gameObject;
                position = position + new Vector2(constellation.GetExtent(), 0);
                toRender.GetComponent<Transform>().position = position;
                toRender.SetActive(true);
                _rightMostConstRBound = constellation.GetRightBound();
                _activeConstellations.Enqueue(toRender.GetComponent<Rigidbody2D>());
            }
        }
    }
}
