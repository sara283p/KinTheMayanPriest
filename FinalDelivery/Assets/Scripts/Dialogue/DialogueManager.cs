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
    private bool _isSentenceFinished;
    private String _currentSentence;

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
        if (!_isSentenceFinished && _currentSentence != null)
        {
            StopAllCoroutines();
            dialogueText.text = _currentSentence;
            _isSentenceFinished = true;
            return;
        }
        
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        StopAllCoroutines();
        _currentSentence = _sentences.Dequeue();
        StartCoroutine(TypeSentence(_currentSentence));
    }

    private IEnumerator TypeSentence(String sentence)
    {
        _isSentenceFinished = false;
        dialogueText.text = "";
        foreach (char character in sentence)
        {
            dialogueText.text += character;
            yield return null;
        }

        _isSentenceFinished = true;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        print("End of conversation");
        EventManager.TriggerEvent("EndOfDialogue");
    }
}
