using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
   public GameObject levelSelectionCanvas;
   public GameObject introCanvas;
   public Dialogue intro;

   private bool _isShowingIntro;
   private String _selectedLevel;
   
   public void LoadSelectedLevel(string levelName)
   {
      _selectedLevel = levelName;
      if (GameManager.Instance.IsNewGame(levelName))
      {
         ShowIntro();
      }
      else
      {
         LoadLevel();
      }
      
   }

   private void LoadLevel()
   {
      introCanvas.SetActive(false);
      EventManager.StopListening("EndOfDialogue", LoadLevel);
      GameManager.Instance.ChangeLevel(_selectedLevel, levelSelectionCanvas);
      Time.timeScale = 1f;
   }

   private void ShowIntro()
   {
      levelSelectionCanvas.SetActive(false);
      introCanvas.SetActive(true);
      DialogueManager.Instance.StartDialogue(intro);
      EventManager.StartListening("EndOfDialogue", LoadLevel);
      _isShowingIntro = true;
   }

   private void Update()
   {
      if (!_isShowingIntro)
      {
         if (InputManager.GetButtonDown("Button1"))
         {
            SceneManager.LoadScene("Scenes/SavedDataMenu");
         }
      }
      else
      {
         if (InputManager.GetButtonDown("Button0"))
         {
            DialogueManager.Instance.NextSentence();
         }

         if (InputManager.GetButtonDown("Button1"))
         {
            DialogueManager.Instance.EndDialogue();
         }
      }
   }
}
