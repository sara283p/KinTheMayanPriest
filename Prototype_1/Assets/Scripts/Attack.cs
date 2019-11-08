using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Locker locker;

    public LineRenderer lineRenderer;
    
    private bool _locked;
    private bool _attack;
    private bool _isAttackOngoing;
    
    private List<Star> _selectedStars = new List<Star>();
    
    private void Awake()
    // Initialize the attack effect
    {
        lineRenderer.positionCount = 0;
    }
    
    void Update()
    {
        if (Input.GetButtonDown("LockStarAttack"))
        {
            _locked = true;
        }
        
        if (Input.GetButtonDown("Attack"))
        {
            _attack = true;
        }
    }
    
    private void TargetStar()
    {
        var targetedStar = locker.GetTargetedStar();

        if (targetedStar)
        {
            if (targetedStar.IsSelected() == false)
            {
                targetedStar.Select();
                _selectedStars.Add(targetedStar);

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(_selectedStars.Count - 1, targetedStar.transform.position);
                
            }
        }
        else
        {
            foreach (Star star in _selectedStars)
            {
                star.Deselect();
            }
            
            lineRenderer.positionCount = 0;
            _selectedStars.Clear();
        }
        
    }

    private void PerformAttack()
    {
        Enemy targetedEnemy = locker.GetTargetedEnemy(_selectedStars[_selectedStars.Count - 1]);

        if (targetedEnemy)
        {
            StartCoroutine(AttackEffect(targetedEnemy));
            targetedEnemy.TakeDamage(_selectedStars.Count*10);
            print("Inflicted: " + _selectedStars.Count*10);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
        
        foreach (var star in _selectedStars)
        {
            star.Deselect();
        }
        _selectedStars.Clear();
        
    }

    IEnumerator AttackEffect(Enemy enemy)
    {
        _isAttackOngoing = true;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(_selectedStars.Count, enemy.transform.position);

        yield return new WaitForSeconds(2f);
        
        if (_isAttackOngoing)
        {
            _isAttackOngoing = false;
            lineRenderer.positionCount = 0;
        }
    }

    private void FixedUpdate()
    {
        if (_locked)
        {
            // If an attack animation is ongoing, reset the lineRenderer
            if (_isAttackOngoing)
            {
                _isAttackOngoing = false;
                lineRenderer.positionCount = 0;
            }
            _locked = false;
            TargetStar();
        }

        if (_attack)
        {
            _attack = false;
            if (_selectedStars.Count > 0) PerformAttack();
        }
    }
}
