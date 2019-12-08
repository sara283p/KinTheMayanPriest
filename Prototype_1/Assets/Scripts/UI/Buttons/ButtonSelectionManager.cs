using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelectionManager : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI selectedButtonText;
    
    public void OnSelect(BaseEventData eventData)
    {
        selectedButtonText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        selectedButtonText.color = Color.yellow;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectedButtonText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        selectedButtonText.color = Color.white;
    }
}
