using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private bool hasJustStarted = false;
    private bool isMarkerOnYes;
    private bool isMarkerOnNo;
    public GameObject YesMarker;
    public GameObject NoMarker;
    
    void Start()
    {
        YesMarker = GameObject.FindGameObjectWithTag("YesMarker");
        NoMarker = GameObject.FindGameObjectWithTag("NoMarker");

        hasJustStarted = true;
    }
    
    void Update()
    {
        if (hasJustStarted)
        {
            hasJustStarted = false;
            YesMarker.SetActive(false);
            isMarkerOnYes = false;
            isMarkerOnNo = true;
        }
        
        else if (!isMarkerOnYes && isMarkerOnNo && InputManager.GetAxis("Horizontal") < -0.01)
        {
            NoMarker.SetActive(false);
            isMarkerOnNo = false;
            YesMarker.SetActive(true);
            isMarkerOnYes = true;
        }
        
        else if (!isMarkerOnNo && isMarkerOnYes && InputManager.GetAxis("Horizontal") > 0.01)
        {
            YesMarker.SetActive(false);
            isMarkerOnYes = false;
            NoMarker.SetActive(true);
            isMarkerOnNo = true;
        }
    }
}
