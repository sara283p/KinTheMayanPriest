using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isDisabled;
    private bool _isInCooldown;
    public float damagePoints;
    public float coolDownTime = 10;
    public float coolDownOpacity = 0.3f;
    public GameObject selectedForAttack;
    private Color _color;
    private SpriteRenderer _renderer;
    private bool _isMovable;
    private CircleCollider2D _collider;
    private float _starRadius = 0.285f;
    public LayerMask _foregroundLayerMask;
    private Rigidbody2D _rb;
    [SerializeField] private Vector2[] _containmentCheck = new Vector2[4];
    private bool _starContact;
    
    void Start()
    {
        selectedForAttack.SetActive(false);
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _isMovable = GetComponent<MovableStar>();
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        damagePoints = GameManager.Instance.enemyPerStarDamage;
        coolDownTime = GameManager.Instance.starCooldownTime;
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
        LayerMask starLayer = 1 << LayerMask.NameToLayer("Star");
        
        List<Star> inRangeMovableStars;
        
        inRangeMovableStars = Physics2D.OverlapCircleAll(_rb.position, _collider.radius, starLayer)
            .Select(coll => coll.GetComponent<Star>())
            .Where(s => s._isMovable)
            .ToList();
        
        DarkenStar();
        if (!_isMovable)
        {
            foreach (Star star in inRangeMovableStars)
            {
                star._starContact = false;
                star.BrightenStar();
            }
        }
        
        _isInCooldown = true;
        yield return new WaitForSeconds(coolDownTime);
        
        inRangeMovableStars = Physics2D.OverlapCircleAll(_rb.position, _collider.radius, starLayer)
            .Select(coll => coll.GetComponent<Star>())
            .Where(s => s._isMovable)
            .ToList();
        
        BrightenStar();
        
        if (!_isMovable)
        {
            foreach (Star star in inRangeMovableStars)
            {
                star._starContact = true;
                star.DarkenStar();
            }
        }

        _isInCooldown = false;
    }
    
    public void HighlightStar()
    {
        Color newColor = new Color(0f, 255f, 0);
        _renderer.color = newColor;
        
    }
    
    public void DeHighlightStar()
    {
        Color newColor = _color;
       _renderer.color = newColor;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isMovable && other.CompareTag("Star"))
        {
            if (!other.GetComponent<Star>().isDisabled)
            {
                _starContact = true;
            }
            else
            {
                _starContact = false;
            }
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
            if(!_isInCooldown) BrightenStar();
        }
    }
}
