using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaUnlockedListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SecretUnlocked", Fade);
    }

    private void Fade()
    {
        StartCoroutine(DestroyAnimation());
    }
    
    IEnumerator DestroyAnimation()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        float time = 0.5f;
        var solidColor = renderers[0].color;
        var transparentColor = solidColor;
        
        float dec = 0.01f;
        
        for (int i = 0; i < 100; i++)
        {
            transparentColor.a -= dec;
            foreach (var r in renderers)
            {
                r.material.color = transparentColor;
            }
            yield return new WaitForSeconds(0.001f);
        }
        
        Destroy(this);
    }
}
