﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputManager _manager;
    private static Control _control;
    private static KeyCode[] _mouseKeyCodes;

    private OperatingSystemFamily _os;

    private void Awake()
    {
        if (!_manager)
        {
            _manager = this;
            if (!_manager)
            {
                print("There must be an active InputManager");
            }
            else
            {
                _mouseKeyCodes = new KeyCode[7];

                _mouseKeyCodes[0] = KeyCode.Mouse0;
                _mouseKeyCodes[1] = KeyCode.Mouse1;
                _mouseKeyCodes[2] = KeyCode.Mouse2;
                _mouseKeyCodes[3] = KeyCode.Mouse3;
                _mouseKeyCodes[4] = KeyCode.Mouse4;
                _mouseKeyCodes[5] = KeyCode.Mouse5;
                _mouseKeyCodes[6] = KeyCode.Mouse6;

            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _control = Control.Keyboard;
        _os = SystemInfo.operatingSystemFamily;
        switch (_os)
        {
            case OperatingSystemFamily.Windows:
                print("I'm on Windows");
                break;
            case OperatingSystemFamily.MacOSX:
                print("I'm on Mac OS");
                break;
            default:
                print("I'm on Linux");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Control previous = _control;
        String[] names = Input.GetJoystickNames().Where(joyName => !joyName.Equals("")).ToArray();
        
        if (names.Length == 0)
        {
            _control = Control.Keyboard;
        }
        else if (!names[0].ToLower().Contains("xbox") && !names[0].ToLower().Contains("x360") && names[0].Contains("Wireless Controller"))
        {
            switch (_os)
            {
                case OperatingSystemFamily.Windows:
                    _control = Control.Ps4;
                    break;
                case OperatingSystemFamily.MacOSX:
                    _control = Control.MacOsPs4;
                    break;
                default:
                    _control = Control.LinuxPs4;
                    break;
            }
        }
        else
        {
            switch (_os)
            {
                case OperatingSystemFamily.Windows:
                    _control = Control.Xbox;
                    break;
                case OperatingSystemFamily.MacOSX:
                    _control = Control.MacOsXbox;
                    break;
                default:
                    _control = Control.LinuxXbox;
                    break;
            }
        }

        if (_control != previous)
        {
            print(_control);
        }
    }

    public static bool GetButtonDown(String name)
    {
        return _control.GetButtonDown(name);
    }

    public static float GetAxis(String name)
    {
        return _control.GetAxis(name);
    }

    public static float GetAxisRaw(String name)
    {
        return _control.GetAxisRaw(name);
    }

    public static bool GetButtonUp(String name)
    {
        return _control.GetButtonUp(name);
    }

    public static bool GetButton(String name)
    {
        return _control.GetButton(name);
    }

    public static bool AnyKeyDown(bool excludeMouse)
    {
        if (excludeMouse)
        {
            foreach (KeyCode key in _mouseKeyCodes)
            {
                if (Input.GetKeyDown(key))
                {
                    return false;
                }
            }
        }

        return Input.anyKeyDown;
    }
}
