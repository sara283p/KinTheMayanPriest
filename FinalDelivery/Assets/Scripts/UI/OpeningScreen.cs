using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    //once a button has been pressed, the "Press a button to play" text becomes yellow for 0.5 s
    //before returning white and then loading the next scene
    void Update()
    {
        if (InputManager.AnyKeyDown(true))
        {
            text.color = Color.yellow;
            StartCoroutine(setTimeout());
        } 
    }

    IEnumerator setTimeout()
    {
        yield return new WaitForSeconds(0.2f);
        text.color = Color.white;
        SceneManager.LoadScene("Scenes/SavedDataMenu");
    }
}