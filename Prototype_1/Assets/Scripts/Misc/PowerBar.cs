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
    private float _disabledStarOpacity;
    private int _enabledStars;

    private void Awake()
    {
        _linkableStars = GameManager.Instance.linkableStars;
        _enabledStars = 0;
        _maxLinkableStars = 4;
        _leftEdge = GetComponentInChildren<PowerBarLeftEdge>().transform.position;
        _rightEdge = GetComponentInChildren<PowerBarRightEdge>().transform.position;
        _powerBarStarsDistance = (_rightEdge.x - _leftEdge.x) / (_maxLinkableStars - 1);
        _line = GetComponent<LineRenderer>();
        _starSignals = new List<GameObject>();
        _disabledStarOpacity = 0.1f;

        Vector2 pos = _leftEdge;
        for (int i = 0; i < _linkableStars; i++)
        {
            AddStar(pos);
            pos.x += _powerBarStarsDistance;
        }
    }

    private void DisableStar(GameObject star)
    {
        SpriteRenderer rend = star.GetComponent<SpriteRenderer>();
        Color starColor = rend.color;
        starColor.a = _disabledStarOpacity;
        rend.color = starColor;
        if (_enabledStars > 0)
        {
            _line.positionCount--;
            _enabledStars--;
        }
    }

    private void EnableStar(GameObject star)
    {
        _line.positionCount++;
        _line.SetPosition(_line.positionCount - 1, star.transform.localPosition);
        SpriteRenderer rend = star.GetComponent<SpriteRenderer>();
        Color starColor = rend.color;
        starColor.a = 1;
        rend.color = starColor;
        _enabledStars++;
    }

    private void AddStar(Vector2 position)
    {
        _starSignals.ForEach(EnableStar);
        GameObject newStar = Instantiate(starPrefab, transform);
        newStar.transform.position = position;
        _starSignals.Add(newStar);
        _starSignals.ForEach(DisableStar);
    }

    private void OnEnable()
    {
        EventManager.StartListening("LinkableStarsIncreased", IncreaseLinkableStars);
        EventManager.StartListening("StarSelected", StarSelected);
        EventManager.StartListening("StarDeselected", StarDeselected);
    }

    private void OnDisable()
    {
        EventManager.StopListening("LinkableStarsIncreased", IncreaseLinkableStars);
        EventManager.StopListening("StarSelected", StarSelected);
        EventManager.StopListening("StarDeselected", StarDeselected);
    }

    private void IncreaseLinkableStars()
    {
        _linkableStars++;
        AddStar((Vector2) _starSignals.LastOrDefault().transform.position + new Vector2(_powerBarStarsDistance, 0));
    }

    private void StarSelected()
    {
        EnableStar(_starSignals.ToArray()[_enabledStars]);
    }

    private void StarDeselected()
    {
        DisableStar(_starSignals.ToArray()[_enabledStars - 1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
