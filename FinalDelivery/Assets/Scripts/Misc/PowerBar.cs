using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerBar : MonoBehaviour
{
    public GameObject starPrefab;
    public GameObject dotPrefab;
    
    private int _linkableStars;
    private int _maxLinkableStars;
    private Vector2 _leftEdge;
    private Vector2 _rightEdge;
    private float _powerBarStarsDistance;
    private int _dotsPerSegment;
    private float _interDotDistance;
    private List<GameObject> _starSignals;
    private float _disabledStarOpacity;
    private int _enabledStars;
    private Vector3 _originalScale;
    private ParticleSystem[] _particleEffects;
    private List<GameObject[]> _segments;

    private void Awake()
    {
        _linkableStars = GameManager.Instance.linkableStars;
        _enabledStars = 0;
        _maxLinkableStars = 3;
        _leftEdge = GetComponentInChildren<PowerBarLeftEdge>().transform.position;
        _rightEdge = GetComponentInChildren<PowerBarRightEdge>().transform.position;
        _powerBarStarsDistance = (_rightEdge.x - _leftEdge.x) / (_maxLinkableStars - 1);
        _dotsPerSegment = 4;
        _interDotDistance = _powerBarStarsDistance / (_dotsPerSegment + 1);
        _starSignals = new List<GameObject>();
        _disabledStarOpacity = 0.3f;
        _originalScale = starPrefab.transform.localScale;
        _particleEffects = GetComponentsInChildren<ParticleSystem>();
        _segments = new List<GameObject[]>();

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
            foreach (var effect in _particleEffects)
            {
                effect.Stop();
            }
        }

        if (_enabledStars > 1)
        {
            DisableSegment(_segments[_enabledStars - 2]);
        }
        if (_enabledStars > 0)
        {
            _enabledStars--;
        }
    }

    private void EnableStar(GameObject star)
    {
        SpriteRenderer rend = star.GetComponent<SpriteRenderer>();
        Color starColor = rend.color;
        starColor.a = 1;
        rend.color = starColor;
        _enabledStars++;
        if (_enabledStars > 1)
        {
            EnableSegment(_segments[_enabledStars - 2]);
            
            if (_enabledStars == _linkableStars)
            {
                foreach (var effect in _particleEffects)
                {
                    effect.transform.position = _starSignals.Last().transform.position;
                    effect.Play();
                }
            }
        }
    }

    private void AddStar(Vector2 position)
    {
        _starSignals.ForEach(EnableStar);
        _starSignals.ForEach(star => star.transform.localScale = _originalScale);
        GameObject newStar = Instantiate(starPrefab, transform);
        Transform newStarTransform = newStar.transform;
        newStarTransform.position = position;
        if (_starSignals.Count > 0)
        {
            newStarTransform.localScale *= 1.5f;
            
            GameObject[] segment = new GameObject[_dotsPerSegment];
            Vector2 newPos = _starSignals.Last().transform.position;
            for (int i = 0; i < _dotsPerSegment; i++)
            {
                newPos.x += _interDotDistance;
                GameObject newDot = Instantiate(dotPrefab, transform);
                newDot.transform.position = newPos;
                segment[i] = newDot;
            }
            _segments.Add(segment);
        }

        _starSignals.Add(newStar);
        _starSignals.ForEach(DisableStar);
        _segments.ForEach(DisableSegment);
    }

    private void DisableSegment(GameObject[] segment)
    {
        foreach (var dot in segment)
        {
            dot.SetActive(false);
        }
    }

    private void EnableSegment(GameObject[] segment)
    {
        foreach (var dot in segment)
        {
            dot.SetActive(true);
        }
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
        _starSignals.ForEach(DisableStar);
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
