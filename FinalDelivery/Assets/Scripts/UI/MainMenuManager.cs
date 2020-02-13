using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void ToLevelSelection()
    {
        SceneManager.LoadScene("Scenes/LevelSelection");
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
