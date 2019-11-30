using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private CircleCollider2D _collider;
    private float _starRadius = 0.385f;
    public LayerMask _foregroundLayerMask;
    private Rigidbody2D _rb;
    [SerializeField] private Vector2[] _containmentCheck = new Vector2[4];
    private bool _starContact;
    
    void Start()
    {
        selectedForAttack.SetActive(false);
        _renderer = GetComponent<Renderer>();
        _color = _renderer.material.color;
        _isMovable = GetComponent<MovableStar>();
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        UpdateContainmentCheck();
    }

    private void UpdateContainmentCheck()
    {
        for (int i = 0; i < _containmentCheck.Length; i++)
        {
            _containmentCheck[i] = _rb.position;
        }

        _containmentCheck[0].x -= _starRadius;
        _containmentCheck[1].x += _starRadius;
        _containmentCheck[2].y -= _starRadius;
        _containmentCheck[3].y += _starRadius;
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
            _starContact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isMovable && other.CompareTag("Star"))
        {
            _starContact = false;
        }
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
            return;
        }

        UpdateContainmentCheck();
        if (_starContact || _containmentCheck
                                .Select(pos => Physics2D.OverlapCircle(pos, 0.01f, _foregroundLayerMask))
                                .Aggregate(true, (res, coll) => res && coll))
        {
            DarkenStar();
        }
        else
        {
            BrightenStar();
        }
    }
}
