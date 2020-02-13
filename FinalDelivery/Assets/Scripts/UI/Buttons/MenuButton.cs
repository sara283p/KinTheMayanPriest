using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : Button
{
    private TextMeshProUGUI _text;
    protected override void Awake()
    {
        base.Awake();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _text.color = Color.yellow;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _text.color = Color.white;
    }
}
