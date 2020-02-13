using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
   public void LoadSelectedLevel(string levelName)
   {
      GameManager.Instance.ChangeLevel(levelName);
      Time.timeScale = 1f;
   }

   private void Update()
   {
      if (InputManager.GetButtonDown("Button1"))
      {
         SceneManager.LoadScene("Scenes/SavedDataMenu");
      }
   }
}
