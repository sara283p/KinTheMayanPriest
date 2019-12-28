using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{

    public TMP_Text stalactiteText;
    [SerializeField] private float timeToAppear = 2f;
    private float timeWhenDisappear;

    
    
    void Update()
    {
        if (stalactiteText.enabled && (Time.time >= timeWhenDisappear))
        {
            stalactiteText.enabled = false;
        }
    }
    
    public void EnableStalactiteText()
    {
        stalactiteText.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("StalactiteApproaching", EnableStalactiteText);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("StalactiteApproaching", EnableStalactiteText);
    }
    
}
