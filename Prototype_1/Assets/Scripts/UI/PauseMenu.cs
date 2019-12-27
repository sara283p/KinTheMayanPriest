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
    public GameObject exitLevelUI;
    
    //elements needed to move the marker
    private bool isResumeSelected;
    private bool isExitSelected;
    private TextMeshProUGUI selectedText;
    [SerializeField] private GameObject resume;
    [SerializeField] private GameObject exit;

    void Start()
    {
        isResumeSelected = true;
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
        if (InputManager.GetButtonDown("Button0") && isResumeSelected && !isExitSelected)
        {
            if (isGamePaused)
            {
                Resume();
            }
        }

        //open exit confirmation UI after pressing on "Exit"
        if (InputManager.GetButtonDown("Button0") && !isResumeSelected && isExitSelected)
        {
            if (isGamePaused)
            {
                exitLevelUI.SetActive(true);
                pauseMenuUI.SetActive(false);
            }
        }
            
        
        //moving upwards between the options
        if (isGamePaused && InputManager.GetAxis("Vertical") > 0.01)
        {
            
            //moving from "Exit" to "Resume"
            if (!isResumeSelected && isExitSelected)
            {
                isExitSelected = false;
                isResumeSelected = true;
                selectedText.color = Color.white;
                selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
                selectedText.color = Color.yellow;
            }
        }
        
        //moving downwards between the options
        else if (isGamePaused && InputManager.GetAxis("Vertical") < -0.01)
        {
            //moving from "Resume" to "Exit"
            if (isResumeSelected && !isExitSelected)
            {
                isResumeSelected = false;
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
        isExitSelected = false;
        selectedText = exit.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.white;
        selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }

}
