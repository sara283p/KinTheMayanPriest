using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private bool isRestartSelected;
    private bool isExitSelected;
    private TextMeshProUGUI selectedText;
    [SerializeField] private GameObject restart;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject gameOverUI;

    void Start()
    {
        isRestartSelected = true;
        isExitSelected = false;
        selectedText = restart.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }
    void Update()
    {
        if (isRestartSelected && InputManager.GetAxis("Vertical") < -0.01)
        {
            isRestartSelected = false;
            isExitSelected = true;
            selectedText.color = Color.white;
            selectedText = exit.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }
        
        else if (isExitSelected && InputManager.GetAxis("Vertical") > 0.01)
        {
            isExitSelected = false;
            isRestartSelected = true;
            selectedText.color = Color.white;
            selectedText = restart.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }

        if (isRestartSelected && InputManager.GetButtonDown("Button0"))
        {
            gameOverUI.SetActive(false);
            SceneManager.LoadScene("LevelDesignTest_01");
        }

        if (isExitSelected && InputManager.GetButtonDown("Button0"))
        {
            SceneManager.LoadScene("OpeningScreenUI");
        }
    }
}