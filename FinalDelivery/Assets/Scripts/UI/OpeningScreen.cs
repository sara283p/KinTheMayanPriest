using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private KeyCode[] _keys;
    private void Awake()
    {
        _keys = new KeyCode[7];
        
        _keys[0] = KeyCode.Mouse0;
        _keys[1] = KeyCode.Mouse1;
        _keys[2] = KeyCode.Mouse2;
        _keys[3] = KeyCode.Mouse3;
        _keys[4] = KeyCode.Mouse4;
        _keys[5] = KeyCode.Mouse5;
        _keys[6] = KeyCode.Mouse6;
    }

    //once a button has been pressed, the "Press a button to play" text becomes yellow for 0.5 s
    //before returning white and then loading the next scene
    void Update()
    {
        // Ignore any mouse button press
        if (AnyMouseButton())
        {
            return;
        }
        
        if (Input.anyKeyDown)
        {
            text.color = Color.yellow;
            StartCoroutine(setTimeout());
        } 
    }

    private bool AnyMouseButton()
    {
        foreach (KeyCode key in _keys)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }

        return false;

    }

    IEnumerator setTimeout()
    {
        yield return new WaitForSeconds(0.2f);
        text.color = Color.white;
        SceneManager.LoadScene("Scenes/SavedDataMenu");
    }
}