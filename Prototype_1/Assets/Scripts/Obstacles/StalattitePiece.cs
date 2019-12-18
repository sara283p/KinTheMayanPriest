using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalattitePiece : MonoBehaviour
{

    public void Destroy()
    {
        StartCoroutine(DestroyAnimation());
    }

    IEnumerator DestroyAnimation()
    {
        float time = 0.5f;
        var solidColor = GetComponent<SpriteRenderer>().color;
        var transparentColor = solidColor;
//        transparentColor.a = 0.3f;
        
        yield return new WaitForSeconds(3);

//        for (int i = 0; i < 5; i++)
//        {
//            GetComponent<SpriteRenderer>().material.color = transparentColor;
//            yield return new WaitForSeconds(time);
//            GetComponent<SpriteRenderer>().material.color = solidColor;
//        }
//        GetComponent<SpriteRenderer>().material.color = transparentColor;

        float dec = 0.01f;
        
        for (int i = 0; i < 100; i++)
        {
            transparentColor.a -= dec;
            GetComponent<SpriteRenderer>().material.color = transparentColor;
            yield return new WaitForSeconds(0.001f);
        }

    }
}
