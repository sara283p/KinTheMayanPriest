using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelManager : MonoBehaviour
{
    [SerializeField] private bool isYesSelected;
    [SerializeField] private bool isNoSelected;
    
    private TextMeshProUGUI selectedText;
    
    public GameObject pauseMenuCanvas;
    public GameObject exitLevelCanvas;
    public GameObject pauseMenuUI;
    public GameObject exitLevelUI;

    private PauseMenuManager pauseMenuScript;
    private ExitLevelManager exitLevelScript;
    
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;



    void Awake()
    {
        pauseMenuScript = pauseMenuCanvas.GetComponent<PauseMenuManager>();
        exitLevelScript = exitLevelCanvas.GetComponent<ExitLevelManager>();
        isYesSelected = false;
        isNoSelected = true;
        selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }
    
    void Update()
    {
        if (exitLevelUI.activeSelf && !pauseMenuUI.activeSelf)
        {
            pauseMenuScript.enabled = false;
            
            if (isNoSelected && InputManager.GetAxis("Horizontal") < -0.01)
            {
               isNoSelected = false;
               isYesSelected = true;
               selectedText.color = Color.white;
               selectedText = yesButton.GetComponentInChildren<TextMeshProUGUI>();
               selectedText.color = Color.yellow;
            }
                           
            else if (isYesSelected && InputManager.GetAxis("Horizontal") > 0.01)
            {
               isYesSelected = false;
               isNoSelected = true;
               selectedText.color = Color.white;
               selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
               selectedText.color = Color.yellow;
            }
           
            if (InputManager.GetButtonDown("Button0"))
            {
               //if "NO" is pressed while the exitLevelUI is active:
               if (!isYesSelected && isNoSelected)
               {
                    pauseMenuUI.SetActive(true);
                    exitLevelUI.SetActive(false);
                    pauseMenuScript.enabled = true;
                    exitLevelScript.enabled = false;

               }
               
               //if "YES" is pressed while the exitLevelUI is active:
               else if (!isNoSelected && isYesSelected)
               {
                   AudioManager.instance.Stop();
                   SceneManager.LoadScene("Scenes/SavedDataMenu");
               }
            }
       
            //ignore the pressing of "Start", which would re-open the pause menu
            if (InputManager.GetButtonDown("Button4"))
            {
                return;
            }
       
            //if B is pressed while in the exitLevelUI, the player is brought back to the menu
            if (InputManager.GetButtonDown("Button1"))
            {
               pauseMenuUI.SetActive(true);
               exitLevelUI.SetActive(false);
               pauseMenuScript.enabled = true;
               exitLevelScript.enabled = false;
            }
       
            //ignore the vertical axis input: this would affect the pause menu and in certain cases make the player exit the UI
            if (InputManager.GetAxis("Vertical") > 0.01)
            {
                return;
            }
               
            if (InputManager.GetAxis("Vertical") < -0.01)
            {
                return;
            } 
        }
    }
}
