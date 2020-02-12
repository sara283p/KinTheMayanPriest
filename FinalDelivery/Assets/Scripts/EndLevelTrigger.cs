using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private List<Animator> _godAnimators;
    private bool _alreadyTriggered;
    private PlayerMovement _movement;
    private Rigidbody2D _rb;
    private bool _isDialogueActive;
    
    private static readonly int Activated = Animator.StringToHash("Activated");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public Dialogue dialogue;
    public GameObject dialogueBox;

    private void EndOfDialogue()
    {
        _isDialogueActive = false;
        EventManager.StopListening("EndOfDialogue", EndOfDialogue);
        EventManager.TriggerEvent("LevelFinished");
    }

    private void Awake()
    {
        _godAnimators = GetComponentsInChildren<Animator>().ToList();
        _movement = FindObjectOfType<PlayerMovement>();
        _rb = _movement.GetComponent<Rigidbody2D>();
        dialogueBox.SetActive(false);
    }
    
     private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.isTrigger && !_alreadyTriggered)
            {
                _movement.DisableInput();
                _movement.GetComponent<Animator>().SetFloat(Speed, 0);
                _rb.velocity = Vector3.zero;
                StartCoroutine(AnimationEnabler());
                _alreadyTriggered = true;
            }
        }
     
     private IEnumerator AnimationEnabler()
         {
             _godAnimators
                 .ForEach(contr => contr.SetBool(Activated, true));
             yield return new WaitForSeconds(2);
             dialogueBox.SetActive(true);
             DialogueManager.Instance.StartDialogue(dialogue);
             _isDialogueActive = true;
             EventManager.StartListening("EndOfDialogue", EndOfDialogue);
             print("LevelFinished");
         }

     private void Update()
     {
         if (_isDialogueActive)
         {
             if (InputManager.GetButtonDown("Button0"))
             {
                 DialogueManager.Instance.NextSentence();
             }
         }
     }
}
