using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private GameObject yesMarker;
    private GameObject noMarker;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject yesMarker = GameObject.FindGameObjectWithTag("YesMarker");
        GameObject noMarker = GameObject.FindGameObjectWithTag("NoMarker");
        
        yesMarker.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (yesMarker.GetComponent<Renderer>().enabled == false && InputManager.GetAxis("Horizontal") < -0.01)
        {
            yesMarker.GetComponent<Renderer>().enabled = true;
            noMarker.GetComponent<Renderer>().enabled = false;
        }
        
        else if (noMarker.GetComponent<Renderer>().enabled == false && InputManager.GetAxis("Horizontal") > 0.01)
        {
            noMarker.GetComponent<Renderer>().enabled = true;
            yesMarker.GetComponent<Renderer>().enabled = false;
        }

        else
        {
            return;
        }
    }
}
