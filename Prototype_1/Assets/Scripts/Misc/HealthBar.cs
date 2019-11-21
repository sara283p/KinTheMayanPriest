using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HealthBar : MonoBehaviour
{
    private PlayerHealth _health;
    private LineRenderer _renderer;
    private float _maxHealth;
    private Vector2 _rightBarMargin;
    private Vector2 _leftBarMargin;
    private float _barLength;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _health = GetComponent<Transform>().parent.parent.GetComponentInChildren<PlayerHealth>();
        _renderer = GetComponent<LineRenderer>();
        _rightBarMargin = GetComponentInChildren<RightBarMargin>().gameObject.transform.localPosition;
        _leftBarMargin = GetComponentInChildren<LeftBarMargin>().gameObject.transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        _maxHealth = _health.max_health;
        _barLength = (_rightBarMargin - _leftBarMargin).magnitude;
        _renderer.positionCount = 2;
        _renderer.SetPosition(0, _leftBarMargin);
        _renderer.SetPosition(1, _rightBarMargin);
    }

    // Update is called once per frame
    void Update()
    {
        float curHealth = _health.cur_health;
        Vector2 rightPosition = _renderer.GetPosition(1);
        if (curHealth <= 0 && Math.Abs(_leftBarMargin.x  - rightPosition.x) <= 0.1)
        {
            _renderer.enabled = false;
            return;
        }
        _renderer.enabled = true;
        float healthPercentage = curHealth / _maxHealth;
        Color color = GetColor(healthPercentage);

        Vector2 newPosition = rightPosition;
        newPosition.x = _leftBarMargin.x + (_barLength * healthPercentage);
        newPosition = Vector3.SmoothDamp(rightPosition, newPosition, ref _velocity, 0.3f);

        _renderer.startColor = _renderer.endColor = color;
        _renderer.SetPosition(1, newPosition);
    }

    private Color GetColor(float percentage)
    {
        if (percentage > 0.7f)
        {
            return Color.green;
        }

        if (percentage < 0.3f)
        {
            return Color.red;
        }

        return Color.yellow;
    }
}
