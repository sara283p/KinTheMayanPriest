using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isInCooldown;
    public int damagePoints = 10;
    public int coolDownTime = 10;
    public float coolDownOpacity = 0.3f;
    public GameObject selectedForAttack;
    
    void Start()
    {
        selectedForAttack.SetActive(false);
    }

    public void Select()
    {
        selectedForAttack.SetActive(true);
    }

    public void Deselect()
    {
        selectedForAttack.SetActive(false);
    }
    
    public bool IsSelected()
    {
        return selectedForAttack.activeInHierarchy;
    }

    public void UseForAttack()
    {
        StartCoroutine(CoolDown());
    }
    
    IEnumerator CoolDown()
    {
        isInCooldown = true;

        var material = GetComponent<Renderer>().material;
        Color newColor;
        Color oldColor = newColor = material.color;
        newColor.a = coolDownOpacity;
        
        material.color = newColor;
       
        yield return new WaitForSeconds(coolDownTime);

        material.color = oldColor;

        isInCooldown = false;
    }
}
