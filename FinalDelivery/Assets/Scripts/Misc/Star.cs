using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isDisabled;
    public float damagePoints;
    public float coolDownTime = 10;
    public float coolDownOpacity = 0.3f;
    public GameObject selectedForAttack;
    public Color highlightColor;
    
    private Color _color;
    private SpriteRenderer _renderer;
    private CircleCollider2D _collider;
    private Rigidbody2D _rb;
    private ParticleSystem _particleEffect;
    
    void Awake()
    {
        selectedForAttack.SetActive(false);
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        damagePoints = GameManager.Instance.enemyPerStarDamage;
        coolDownTime = GameManager.Instance.starCooldownTime;
        _particleEffect = GetComponentInChildren<ParticleSystem>();
        _particleEffect.Stop();
    }

    public void SelectForAttack()
    {
        selectedForAttack.SetActive(true);
        EventManager.TriggerEvent("StarSelected");
    }

    public void DeselectForAttack()
    {
        selectedForAttack.SetActive(false);
        EventManager.TriggerEvent("StarDeselected");
    }
    
    public bool IsSelectedForAttack()
    {
        return selectedForAttack.activeInHierarchy;
    }

    public void UseForAttack()
    {
        StartCoroutine(CoolDown());
        _particleEffect.Play();
    }

    public void DarkenStar()
    {
        isDisabled = true;
        Color newColor = _color;
        newColor.a = coolDownOpacity;
        
        _renderer.material.color = newColor;
        
    }

    public void BrightenStar()
    {
        Color newColor = _color;
        newColor.a = 1;
        
        _renderer.material.color = newColor;
        isDisabled = false;
    }
    
    IEnumerator CoolDown()
    {
        DarkenStar();
        
        yield return new WaitForSeconds(coolDownTime);
        
        BrightenStar();
    }
    
    public void HighlightStar()
    {
        Color newColor = highlightColor;
        _renderer.color = newColor;
        
    }
    
    public void DeHighlightStar()
    {
        Color newColor = _color;
       _renderer.color = newColor;
        
    }

    private void Update()
    {
        if (_renderer.isVisible)
        {
            _collider.enabled = true;
        }
        else
        {
            _collider.enabled = false;
        }
    }
}
