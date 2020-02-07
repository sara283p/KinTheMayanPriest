using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class PauseMenuManager : MonoBehaviour
{
    //elements needed to manage the "paused" state
    public GameObject pauseMenuCanvas;
    public GameObject exitLevelCanvas;
    public GameObject pauseMenuUI;
    public GameObject exitLevelUI;
    private PauseMenuManager pauseMenuScript;
    private ExitLevelManager exitLevelScript;
    
    //elements needed to avoid taking input commands related to Kin
    public GameObject kinPriest;
    private PlayerMovement playerMovementScript;
    private CharacterController characterControllerScript;
    private Locker_Joystick lockerScript;
    private Grappler_Joystick grapplerScript;
    private Attack_Joystick attackScript;

    //elements needed to move the marker
    private bool isResumeSelected;
    private bool isExitSelected;
    private TextMeshProUGUI selectedText;
    [SerializeField] private GameObject resume;
    [SerializeField] private GameObject exit;

    void Start()
    {
        pauseMenuScript = pauseMenuCanvas.GetComponent<PauseMenuManager>();
        exitLevelScript = exitLevelCanvas.GetComponent<ExitLevelManager>();
        playerMovementScript = kinPriest.GetComponent<PlayerMovement>();
        characterControllerScript = kinPriest.GetComponent<CharacterController>();
        lockerScript = kinPriest.GetComponent<Locker_Joystick>();
        grapplerScript = kinPriest.GetComponent<Grappler_Joystick>();
        attackScript = kinPriest.GetComponent<Attack_Joystick>();
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
            if (pauseMenuUI.activeSelf && !exitLevelUI.activeSelf)
            {
                Resume();
            }
            else if (!pauseMenuUI.activeSelf)
            {
                Pause();
            }
        }

        if (pauseMenuUI.activeSelf && !exitLevelUI.activeSelf)
        {
            //resume gameplay if "A button" is pressed while the game is paused
            if (InputManager.GetButtonDown("Button0") && isResumeSelected && !isExitSelected)
            {
                Resume();
            }

            //open exit confirmation UI after pressing on "Exit"
            if (InputManager.GetButtonDown("Button0") && !isResumeSelected && isExitSelected)
            {
                exitLevelUI.SetActive(true);
                pauseMenuUI.SetActive(false);
                exitLevelScript.enabled = true;
                pauseMenuScript.enabled = false;
            }
            
        
            //moving upwards between the options
            if (InputManager.GetAxis("Vertical") > 0.01)
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
            else if (InputManager.GetAxis("Vertical") < -0.01)
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
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        playerMovementScript.enabled = true;
        characterControllerScript.enabled = true;
        lockerScript.enabled = true;
        grapplerScript.enabled = true;
        attackScript.enabled = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isResumeSelected = true;
        isExitSelected = false;
        selectedText = exit.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.white;
        selectedText = resume.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
        exitLevelScript.enabled = false;
        playerMovementScript.enabled = false;
        characterControllerScript.enabled = false;
        lockerScript.enabled = false;
        grapplerScript.enabled = false;
        attackScript.enabled = false;
    }

}
