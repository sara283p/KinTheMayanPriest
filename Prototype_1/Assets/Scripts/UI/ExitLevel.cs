using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private bool isYesSelected;
    private bool isNoSelected;
    private TextMeshProUGUI selectedText;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
    
    
    
    void Start()
    {
        isYesSelected = false;
        isNoSelected = true;
        selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
        selectedText.color = Color.yellow;
    }
    
    void Update()
    {
        if (isNoSelected && InputManager.GetAxis("Horizontal") < -0.01)
        {
            isNoSelected = false;
            isYesSelected = true;
            selectedText.color = Color.white;
            selectedText = yesButton.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }
        
        else if (isYesSelected && InputManager.GetAxis("Horizontal") > 0.01)
        {
            isYesSelected = false;
            isNoSelected = true;
            selectedText.color = Color.white;
            selectedText = noButton.GetComponentInChildren<TextMeshProUGUI>();
            selectedText.color = Color.yellow;
        }
    }
}
