using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    //elements needed to manage the "paused" state
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    
    //elements needed to move the marker
    private bool isMarker1On;
    private bool isMarker2On;
    private bool isMarker3On;
    public GameObject marker1;
    public GameObject marker2;
    public GameObject marker3;

    void Update()
    {
        //resume gameplay if "Start button" is pressed while the game is paused, pause it otherwise
        if (InputManager.GetButtonDown("Button4"))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        //resume gameplay if "A button" is pressed while the game is paused
        if (InputManager.GetButtonDown("Button0") && isMarker1On && !isMarker2On && !isMarker3On)
        {
            if (isGamePaused)
            {
                Resume();
            }
        }

        //moving upwards between the options
        if (isGamePaused && InputManager.GetAxis("Vertical") > 0.01)
        {
            //moving from "Settings" to "Resume"
            if (!isMarker1On && isMarker2On && !isMarker3On)
            {
                marker2.SetActive(false);
                isMarker2On = false;
                marker1.SetActive(true);
                isMarker1On = true;
            }
            
            //moving from "Exit" to "Settings"
            else if (!isMarker1On && !isMarker2On && isMarker3On)
            {
                marker3.SetActive(false);
                isMarker3On = false;
                marker2.SetActive(true);
                isMarker2On = true;
            }

        }
        
        //moving downwards between the options
        else if (isGamePaused && InputManager.GetAxis("Vertical") < -0.01)
        {
            //moving from "Resume" to "Settings"
            if (isMarker1On && !isMarker2On && !isMarker3On)
            {
                marker1.SetActive(false);
                isMarker1On = false;
                marker2.SetActive(true);
                isMarker2On = true;
            }
            
            //moving from "Settings" to "Exit"
            else if (!isMarker1On && isMarker2On && !isMarker3On)
            {
                marker2.SetActive(false);
                isMarker2On = false;
                marker3.SetActive(true);
                isMarker3On = true;
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        marker1.SetActive(true);
        isMarker1On = true;
        marker2.SetActive(false);
        isMarker2On = false;
        marker3.SetActive(false);
        isMarker3On = false;
    }
}
