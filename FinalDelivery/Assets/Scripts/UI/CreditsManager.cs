using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public GameObject outroCanvas;
    public GameObject creditsCanvas;
    public Dialogue outroSentences;

    private bool _isShowingOutro;
    private TextMeshProUGUI[] _texts;
    private float _deltaAlpha;

    private void Awake()
    {
        outroCanvas.SetActive(true);
        _isShowingOutro = true;
        creditsCanvas.SetActive(false);
        _texts = creditsCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        _deltaAlpha = 0.01f;
        
        foreach (TextMeshProUGUI text in _texts)
        {
            Color currentColor = text.color;
            currentColor.a = 0;
            text.color = currentColor;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Stop();
        DialogueManager.Instance.StartDialogue(outroSentences);
        EventManager.StartListening("EndOfDialogue", EndOfDialogue);
    }

    private void EndOfDialogue()
    {
        EventManager.StopListening("EndOfDialogue", EndOfDialogue);
        _isShowingOutro = false;
        outroCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
        StartCoroutine(ShowCredits());
    }

    private IEnumerator ShowCredits()
    {
        Color currentColor = _texts[0].color;

        while (currentColor.a < 1)
        {
            currentColor.a += _deltaAlpha;

            foreach (TextMeshProUGUI text in _texts)
            {
                text.color = currentColor;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(10);

        StartCoroutine(FadeOutCredits());
    }

    private IEnumerator FadeOutCredits()
    {
        var currentColor = _texts[0].color;
        
        while (currentColor.a > 0)
        {
            currentColor.a -= _deltaAlpha;

            foreach (TextMeshProUGUI text in _texts)
            {
                text.color = currentColor;
            }

            yield return null;
        }

        SceneManager.LoadScene("Scenes/OpeningScreenUI");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShowingOutro)
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
        else
        {
            if (InputManager.AnyKeyDown(true))
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutCredits());
            }
        }
    }
}
