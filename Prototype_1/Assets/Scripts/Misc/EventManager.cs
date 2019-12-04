using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> _events;

    private static EventManager _manager;

    private static EventManager Instance
    {
        get
        {
            if (_manager == null)
            {
                _manager = FindObjectOfType<EventManager>();

                if (_manager == null)
                {
                    Debug.LogError("There must be an active EventManager!");
                }
                else
                {
                    _manager.Init();
                }
            }

            return _manager;
        }
    }

    void Init()
    {
        if (_events == null)
        {
            _events = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        EventManager instance = Instance;
        if(Instance._events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance._events.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance._events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
            // Add reference count to reduce memory usage?
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (Instance._events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
