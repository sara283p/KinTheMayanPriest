using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public MovingPlatformActivator[] activators;
    public float minSpeed;
    public float speed;

    private bool _isActivated;
    private bool _changeSegment;
    private bool _changeTransition;
    private int _currentActiveSegment;
    private Vector2 _startToEnd;
    private Vector2 _endToStart;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private Vector2 _currentDirection;
    private Transform _tr;
    private Vector2 _initialPosition;
    private StartPos _startPosObject;
    private EndPos _endPosObject;
    private Coroutine _runningRoutine;
    private float _starCooldown;
    private bool _interrupted;

    private void Awake()
    {
        _tr = GetComponent<Transform>();
        _starCooldown = GameManager.Instance.starCooldownTime;
        _initialPosition = _tr.position;
        Init();
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerDeath", Init);
    }

    void FixedUpdate()
    {
        if (!_isActivated)
        {
            return;
        }

        Vector2 pos = transform.position;
        float distance = Math.Min(_startPosObject.GetDistance(), _endPosObject.GetDistance());

        // Compute distance as a percentage having value 1.5 when the platform is in the middle of the moving path
        distance = distance * 3 / (_startPos - _endPos).magnitude;
        

        float currentSpeed = Math.Min(speed, speed * distance);
        if (currentSpeed < minSpeed)
        {
            currentSpeed = minSpeed;
        }

        Vector2 delta = currentSpeed * Time.fixedDeltaTime * _currentDirection;
        _tr.position += (Vector3) delta;
    }

    public void Activate()
    {
        if (!_isActivated)
        {
            _isActivated = true;
            _runningRoutine = StartCoroutine(DisablingTimer());
        }
        else
        {
            _changeSegment = true;
        }
    }

    private IEnumerator DisablingTimer()
    {
        yield return new WaitForSeconds(_starCooldown);
        _interrupted = true;
    }

    private void Init()
    {
        _isActivated = false;
        _tr.position = _initialPosition;
        
        _startPosObject = activators[0].GetComponentInChildren<StartPos>();
        _endPosObject = activators[0].GetComponentInChildren<EndPos>();
        _startPos = _startPosObject.transform.position;
        _endPos = _endPosObject.transform.position;
        _startToEnd = (_endPos - _startPos).normalized;
        _endToStart = -_startToEnd;
        _currentDirection = _startToEnd;
        _currentActiveSegment = 0;

        // Disable all MovingPlatformSegmentEnd components
        activators
            .Aggregate(new List<MovingPlatformSegmentEnd>(), (init, activator) => init.Concat(activator.GetComponentsInChildren<MovingPlatformSegmentEnd>()).ToList())
            .ForEach(end => end.enabled = false);
        
        foreach (var activator in activators)
        {
            activator.gameObject.layer = LayerMask.NameToLayer("DestructibleObstacle");
        }
        
        // Enable only the MovingPlatformSegmentEnd components of the first activator
        foreach (MovingPlatformSegmentEnd end in activators[0].GetComponentsInChildren<MovingPlatformSegmentEnd>())
        {
            end.enabled = true;
        }
    }

    public void EndReached()
    {
        if (_currentDirection == _endToStart)
        {
            if (_interrupted)
            {
                _isActivated = false;
                _interrupted = false;
                activators[_currentActiveSegment].gameObject.layer = LayerMask.NameToLayer("DestructibleObstacle");
                _currentDirection = _startToEnd;
            }
            else
            {
                _currentDirection = _startToEnd;
            }
        }
        else
        {
            if (!_changeSegment)
            {
                _currentDirection = _endToStart;
            }
            else
            {
                ChangeSegment();
            }
        }
    }

    private void ChangeSegment()
    {
        if (_runningRoutine != null)
        {
            StopCoroutine(_runningRoutine);
        }
        _changeSegment = false;
        _runningRoutine = StartCoroutine(DisablingTimer());
        MovingPlatformActivator currentActivator = activators[_currentActiveSegment];
        currentActivator.GetComponentInChildren<StartPos>().enabled = false;
        currentActivator.GetComponentInChildren<EndPos>().enabled = false;
        _currentActiveSegment++;
        currentActivator = activators[_currentActiveSegment];
        currentActivator.GetComponentInChildren<StartPos>().enabled = true;
        currentActivator.GetComponentInChildren<EndPos>().enabled = true;
        _startPosObject = currentActivator.GetComponentInChildren<StartPos>();
        _startPos = _startPosObject.transform.position;
        _endPosObject = currentActivator.GetComponentInChildren<EndPos>();
        _endPos = _endPosObject.transform.position;
        _startToEnd = (_endPos - _startPos).normalized;
        _endToStart = -_startToEnd;
        _currentDirection = _startToEnd;
    }
}
