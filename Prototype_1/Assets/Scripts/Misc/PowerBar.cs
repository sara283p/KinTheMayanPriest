using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerBar : MonoBehaviour
{
    public GameObject starPrefab;
    
    private int _linkableStars;
    private int _maxLinkableStars;
    private Vector2 _leftEdge;
    private Vector2 _rightEdge;
    private float _powerBarStarsDistance;
    private LineRenderer _line;
    private List<GameObject> _starSignals;

    private void Awake()
    {
        _linkableStars = GameManager.Instance.linkableStars;
        _maxLinkableStars = 4;
        _leftEdge = GetComponentInChildren<PowerBarLeftEdge>().transform.position;
        _rightEdge = GetComponentInChildren<PowerBarRightEdge>().transform.position;
        _powerBarStarsDistance = (_rightEdge.x - _leftEdge.x) / (_maxLinkableStars - 1);
        _line = GetComponent<LineRenderer>();
        _starSignals = new List<GameObject>();
        AddStar(_leftEdge);
    }

    private void AddStar(Vector2 position)
    {
        GameObject newStar = Instantiate(starPrefab, transform);
        newStar.transform.position = position;
        _starSignals.Add(newStar);
    }

    private void OnEnable()
    {
        EventManager.StartListening("LinkableStarsIncreased", IncreaseLinkableStars);
    }

    private void OnDisable()
    {
        EventManager.StopListening("LinkableStarsIncreased", IncreaseLinkableStars);
    }

    private void IncreaseLinkableStars()
    {
        _linkableStars++;
        AddStar((Vector2) _starSignals.LastOrDefault().transform.position + new Vector2(_powerBarStarsDistance, 0));
        _line.positionCount = _starSignals.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
