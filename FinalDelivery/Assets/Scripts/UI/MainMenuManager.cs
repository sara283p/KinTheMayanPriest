using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject exitMenuCanvas;
    public GameObject newGameCanvas;

    public GameObject mainMenuDefault;
    public GameObject exitMenuDefault;
    public GameObject newGameDefault;

    private EventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
        InitializeCanvases();
    }

    public void ToLevelSelection()
    {
        SceneManager.LoadScene("Scenes/LevelSelection");
    }

    private void InitializeCanvases()
    {
        exitMenuCanvas.SetActive(false);
        newGameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        _eventSystem.SetSelectedGameObject(null);
        InitializeCanvases();
        _eventSystem.SetSelectedGameObject(mainMenuDefault);
    }

    public void ConfirmNewGame()
    {
        _eventSystem.SetSelectedGameObject(null);
        mainMenuCanvas.SetActive(false);
        newGameCanvas.SetActive(true);
        _eventSystem.SetSelectedGameObject(newGameDefault);
    }

    public void ConfirmExit()
    {
        _eventSystem.SetSelectedGameObject(null);
        mainMenuCanvas.SetActive(false);
        exitMenuCanvas.SetActive(true);
        _eventSystem.SetSelectedGameObject(exitMenuDefault);
    }

    public void NewGame()
    {
        GameManager.Instance.ReinitializePersistentData();
        ToLevelSelection();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}