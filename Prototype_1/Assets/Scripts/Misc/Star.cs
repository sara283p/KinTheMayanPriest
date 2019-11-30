using System;
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
    private Color _color;
    private Renderer _renderer;
    private bool _isMovable;
    
    void Start()
    {
        selectedForAttack.SetActive(false);
        _renderer = GetComponent<Renderer>();
        _color = _renderer.material.color;
        _isMovable = GetComponent<MovableStar>();
    }

    public void SelectForAttack()
    {
        selectedForAttack.SetActive(true);
    }

    public void DeselectForAttack()
    {
        selectedForAttack.SetActive(false);
    }
    
    public bool IsSelectedForAttack()
    {
        return selectedForAttack.activeInHierarchy;
    }

    public void UseForAttack()
    {
        StartCoroutine(CoolDown());
    }

    public void DarkenStar()
    {
        isInCooldown = true;
        Color newColor = _color;
        newColor.a = coolDownOpacity;
        
        _renderer.material.color = newColor;
        
    }

    public void BrightenStar()
    {
        Color newColor = _color;
        newColor.a = 1;
        
        _renderer.material.color = newColor;
        isInCooldown = false;
    }
    
    IEnumerator CoolDown()
    {
        DarkenStar();
        yield return new WaitForSeconds(coolDownTime);
        BrightenStar();
    }
    
    public void HighlightStar()
    {
        Color newColor = new Color(0f, 255f, 0);
        _renderer.material.color = newColor;
        
    }
    
    public void DeHighlightStar()
    {
        Color newColor = _color;
       _renderer.material.color = newColor;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isMovable && other.CompareTag("Star"))
        {
            DarkenStar();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isMovable && other.CompareTag("Star"))
        {
            BrightenStar();
        }
    }
}
