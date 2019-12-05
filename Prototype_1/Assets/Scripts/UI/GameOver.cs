using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
   private bool hasJustStarted = false;
   private bool isMarker1On;
   private bool isMarker2On;
   public GameObject marker1; 
   public GameObject marker2;

   void Start()
   {
       hasJustStarted = true;
   }
    void Update()
    {
        if (hasJustStarted)
        {
            hasJustStarted = false;
            marker2.SetActive(false);
            isMarker2On = false;
            isMarker1On = true;
        }
        
        else if (!isMarker2On && isMarker1On && InputManager.GetAxis("Vertical") < -0.01)
        {
            marker1.SetActive(false);
            isMarker1On = false;
            marker2.SetActive(true);
            isMarker2On = true;
        }
        
        else if (!isMarker1On && isMarker2On && InputManager.GetAxis("Vertical") > 0.01)
        {
            marker2.SetActive(false);
            isMarker2On = false;
            marker1.SetActive(true);
            isMarker1On = true;
        }
    }
}
