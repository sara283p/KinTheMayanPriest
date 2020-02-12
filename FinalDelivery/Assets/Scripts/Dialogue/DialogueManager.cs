using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public static DialogueManager Instance => _instance;

    private static DialogueManager _instance;
    private Queue<String> _sentences;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            
            if (_instance == null)
            {
                Debug.LogError("There must be an active DialogueManager");
            }
            else
            {
                _sentences = new Queue<String>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.name;
        _sentences.Clear();
        foreach (var sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        NextSentence();
    }

    public void NextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(_sentences.Dequeue()));
    }

    private IEnumerator TypeSentence(String sentence)
    {
        dialogueText.text = "";
        foreach (char character in sentence)
        {
            dialogueText.text += character;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        print("End of conversation");
        EventManager.TriggerEvent("EndOfDialogue");
    }
}
