using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class PauseMenu : MonoBehaviour
{
    //elements needed to manage the "paused" state
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    
    //elements needed to move the marker
    private bool isResumeSelected;
    private bool isSettingsSelected;
    private bool isExitSelected;
    private TextMeshProUGUI selectedText;
    [SerializeField] private GameObject resume;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject exit;

    void Start()
    {
        isResumeSelected = true;
        isSettingsSelected = false;
        isExitSelected = false;
        selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
        
    }

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
        if (InputManager.GetButtonDown("Button0") && isResumeSelected && !isSettingsSelected && !isExitSelected)
        {
            if (isGamePaused)
            {
                Resume();
            }
        }
        
        float axisValue = InputManager.GetAxis("Vertical");
        if (axisValue > 0)
        {
            axisValue = 0.15f;
        }
        
        else if (axisValue < 0)
        {
            axisValue = -0.15f;
        }

        //moving upwards between the options
        if (isGamePaused && axisValue > 0.01)
        {
            //moving from "Settings" to "Resume"
            if (!isResumeSelected && isSettingsSelected && !isExitSelected)
            {
                isSettingsSelected = false;
                isResumeSelected = true;
                selectedText.color = Color.white;
                selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
                selectedText.color = Color.yellow;
            }
            
            //moving from "Exit" to "Settings"
            else if (!isResumeSelected && !isSettingsSelected && isExitSelected)
            {
                isExitSelected = false;
                isSettingsSelected = true;
                selectedText.color = Color.white;
                selectedText = settings.GetComponentInChildren<TextMeshProUGUI>();
                selectedText.color = Color.yellow;
            }
        }
        
        //moving downwards between the options
        else if (isGamePaused && axisValue < -0.01)
        {
            //moving from "Resume" to "Settings"
            if (isResumeSelected && !isSettingsSelected && !isExitSelected)
            {
                isResumeSelected = false;
                isSettingsSelected = true;
                selectedText.color = Color.white;
                selectedText = settings.GetComponentInChildren<TextMeshProUGUI>();
                selectedText.color = Color.yellow;
            }
            
            //moving from "Settings" to "Exit"
            else if (!isResumeSelected && isSettingsSelected && !isExitSelected)
            {
                isSettingsSelected = false;
                isExitSelected = true;
                selectedText.color = Color.white;
                selectedText = exit.GetComponentInChildren<TextMeshProUGUI>();
                selectedText.color = Color.yellow;
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
        isResumeSelected = true;
        isSettingsSelected = false;
        isExitSelected = false;
        selectedText = exit.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.white;
        selectedText = settings.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.white;
        selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }

}
