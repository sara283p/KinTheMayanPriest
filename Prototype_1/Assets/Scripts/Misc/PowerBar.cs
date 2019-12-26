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
    private Vector3 _originalScale;
    private ParticleSystem _particleEffect;

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
        _disabledStarOpacity = 0.3f;
        _originalScale = new Vector3(1, 1, 1);
        _particleEffect = GetComponentInChildren<ParticleSystem>();

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
        if (_enabledStars == _linkableStars)
        {
            _particleEffect.Stop();
        }
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
        if (_enabledStars > 1 && _enabledStars == _linkableStars)
        {
            _particleEffect.transform.position = _starSignals.Last().transform.position;
            _particleEffect.Play();
        }
    }

    private void AddStar(Vector2 position)
    {
        _starSignals.ForEach(EnableStar);
        _starSignals.ForEach(star => star.transform.localScale = _originalScale);
        GameObject newStar = Instantiate(starPrefab, transform);
        Transform newStarTransform = newStar.transform;
        newStarTransform.position = position;
        if(_starSignals.Count > 0)
            newStarTransform.localScale *= 2;
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
        AddStar((Vector2) _starSignals.Last().transform.position + new Vector2(_powerBarStarsDistance, 0));
    }

    private void StarSelected()
    {
        EnableStar(_starSignals.ToArray()[_enabledStars]);
    }

    private void StarDeselected()
    {
        if(_enabledStars > 0)
            DisableStar(_starSignals.ToArray()[_enabledStars - 1]);
    }
}
