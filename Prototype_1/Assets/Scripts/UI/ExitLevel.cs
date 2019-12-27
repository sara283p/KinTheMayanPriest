using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    [SerializeField] private bool isYesSelected;
    [SerializeField] private bool isNoSelected;
    
    private TextMeshProUGUI selectedText;
    
    public GameObject pauseMenuUI;
    public GameObject exitLevelUI;
    
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;



    void Start()
    {
        isYesSelected = false;
        isNoSelected = true;
        selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }
    
    void Update()
    {
        if (exitLevelUI.activeSelf && isNoSelected && InputManager.GetAxis("Horizontal") < -0.01)
        {
            isNoSelected = false;
            isYesSelected = true;
            selectedText.color = Color.white;
            selectedText = yesButton.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }
            
        else if (exitLevelUI.activeSelf && isYesSelected && InputManager.GetAxis("Horizontal") > 0.01)
        {
            isYesSelected = false;
            isNoSelected = true;
            selectedText.color = Color.white;
            selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }
        
        if (InputManager.GetButtonDown("Button0") && exitLevelUI.activeSelf)
        {
            //if "NO" is pressed while the exitLevelUI is active:
            if (!isYesSelected && isNoSelected)
            {
                 pauseMenuUI.SetActive(true);
                 exitLevelUI.SetActive(false);
            }
            
            //if "YES" is pressed while the exitLevelUI is active:
            else if (!isNoSelected && isYesSelected)
            {
                SceneManager.LoadScene("Scenes/OpeningScreenUI");
            }
        }

        //if B is pressed while in the exitLevelUI, the player is brought back to the menu
        if (InputManager.GetButtonDown("Button1") && exitLevelUI.activeSelf)
        {
            pauseMenuUI.SetActive(true);
            exitLevelUI.SetActive(false);
        }
    }
}
