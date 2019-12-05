using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyRotation : MonoBehaviour
{
    public int constellationCount;

    private ConstellationSet _constellations;
    private Queue<Transform> _activeConstellations;
    private float _firstRowHeight;
    private float _leftLimit;
    [SerializeField] private float _minDistance;
    private float _rightLimit;
    private float _bottomLimit;
    private bool _rotate;
    [SerializeField] public float _speed;
    private float _rightMostConstRBound;

    public bool debugMode;
    public bool manualMode;


    private void GenerateConstellations()
    {
        //We must also add gods' constellations to the set, when needed
        for (int i = 0; i < constellationCount; i++)
        {
            GameObject newObject;
            if (!debugMode)
            {
                newObject = ConstellationGenerator.Instance.GenerateConstellation();
            }
            else
            { 
                newObject = ConstellationGenerator.Instance.GenerateDebuggingConstellation();
            }
            newObject.transform.SetParent(GetComponent<Transform>());
            
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
        _activeConstellations = new Queue<Transform>();
        _rightLimit = GetComponentInChildren<SkyRightLimit>().GetComponent<Transform>().position.x;
        _leftLimit = GetComponentInChildren<SkyLeftLimit>().GetComponent<Transform>().position.x;
        _bottomLimit = GetComponentInChildren<SkyBottomLimit>().GetComponent<Transform>().position.y;
        _firstRowHeight = GetComponentInChildren<SkyFirstRowHeight>().GetComponent<Transform>().position.y;
        // Values used for testing
        /*_height = 5f;
        _margin = 7f;
        _minDistance = 4f;
        _rightMargin = 53f;
        Camera mainCamera = Camera.main;
        Vector2 screenBottomLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);*/
        _rightMostConstRBound = _leftLimit;

    }

    private void RotateInput()
    {
        if (InputManager.GetButtonDown("Button2"))
        {
            _rotate = true;
        }
        else if (InputManager.GetButtonUp("Button2"))
        {
            _rotate = false;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateConstellations();
        float newX = _rightMostConstRBound;
        float newY = _firstRowHeight;
        Constellation constellation;

        if (!manualMode)
        {
            do
            {
                do
                {
                    Vector2 position = new Vector2(newX, newY);
                    constellation = _constellations.GetRandomConstellation();
                    GameObject toRender = constellation.gameObject;
                    toRender.GetComponent<Transform>().position = position;
                    toRender.SetActive(true);
                    _activeConstellations.Enqueue(toRender.GetComponent<Transform>());
                    newY = constellation.GetBottomBound() - constellation.GetVerticalExtent() - _minDistance;
                } while (newY > _bottomLimit);

                newY = _firstRowHeight;
                newX = constellation.GetRightBound() + constellation.GetHorizontalExtent() + _minDistance;
            } while (newX < _rightLimit);


            _rightMostConstRBound = constellation.GetRightBound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateInput();
        if (_rotate)
        {
            Vector3 delta = new Vector3(- _speed * Time.deltaTime, 0);

            if (!manualMode)
            {
                foreach (Transform constellation in _activeConstellations)
                {
                    constellation.position += delta;
                }
                
                _rightMostConstRBound += delta.x;
            }
            else
            {
                GetComponentInChildren<Constellation>().transform.position += delta;
            }

            Vector2 position = new Vector2(_rightMostConstRBound + _minDistance, _firstRowHeight);
            if (position.x < _rightLimit)
            {
                bool firstRow = true;
                while (position.y > _bottomLimit)
                {
                    Constellation constellation = _constellations.GetRandomConstellation();
                    if (!constellation)
                        return;
                    GameObject toRender = constellation.gameObject;
                    if (firstRow)
                    {
                        position = position + new Vector2(constellation.GetHorizontalExtent(), 0);
                        firstRow = false;
                    }
                    toRender.GetComponent<Transform>().position = position;
                    toRender.SetActive(true);
                    _rightMostConstRBound = constellation.GetRightBound();
                    _activeConstellations.Enqueue(toRender.GetComponent<Transform>());
                    position.y = constellation.GetBottomBound() - constellation.GetVerticalExtent() - _minDistance;
                }
            }
        }
    }
}