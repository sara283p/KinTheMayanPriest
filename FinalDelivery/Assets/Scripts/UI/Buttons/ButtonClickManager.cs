using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonClickManager : MonoBehaviour
{
    private TextMeshProUGUI pressedButtonText;
    
    public void ResumeClick()
    {
        pressedButtonText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        pressedButtonText.fontSize = 20;
        StartCoroutine(waitAndResize());
    }

    public void SettingsClick()
    {
        pressedButtonText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        pressedButtonText.fontSize = 20;
        StartCoroutine(waitAndResize());
    }

    public void ExitClick()
    {
        pressedButtonText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        pressedButtonText.fontSize = 20;
        StartCoroutine(waitAndResize());
    }

    IEnumerator waitAndResize()
    {
        yield return new WaitForSeconds(0.1f);
        pressedButtonText.fontSize = 24;
    }
}
