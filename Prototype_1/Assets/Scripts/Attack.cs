using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public LayerMask starLayerMask;
    public LayerMask enemyLayerMask;
    
    private bool _locked;
    private bool _attack;
    private GameObject _target;
    private List<Star> _selectedStars = new List<Star>();
    private float _distance;
    public GameObject player;
    public GameObject starViewFinder;

    private Vector3 _direction;

    // Update is called once per frame
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
        // Get viewfinder position as a Vector2
        Vector2 viewFinderPosition = starViewFinder.transform.position;
        Vector2 playerPosition = player.transform.position;

        // Cast a ray from player to viewfinder position
        Vector2 relativeViewFinderPosition = viewFinderPosition - playerPosition;
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, relativeViewFinderPosition, Mathf.Infinity, starLayerMask);

        // If the ray hits a star, put it in the selected stars for attack.
        // Else, empties the list (consider it as a discard attack).
        if (hit.rigidbody != null)
        {
            Star star = (Star) hit.rigidbody.gameObject.GetComponent(typeof(Star));
            if (star != null)
            {
                if (star.IsSelected() == false)
                {
                    star.Select();
                    _selectedStars.Add(star);
                }
                
            }
        }
        else
        {
            foreach (Star star in _selectedStars)
            {
                star.Deselect();
                _selectedStars.Remove(star);
            }
        }

        _locked = false;
    }

    private void PerformAttack()
    {
        // Get viewfinder position as a Vector2
        Vector2 viewFinderPosition = starViewFinder.transform.position;
        Vector2 playerPosition = player.transform.position;

        // Cast a ray from player to viewfinder position
        Vector2 relativeViewFinderPosition = viewFinderPosition - playerPosition;
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, relativeViewFinderPosition, Mathf.Infinity, enemyLayerMask);

        // If the ray hits an enemy, perform the attack.
        // Else, empties the list (consider it as a discard attack).
        if (hit.rigidbody != null)
        {
            Enemy enemy = (Enemy) hit.rigidbody.gameObject.GetComponent(typeof(Enemy));
            if (enemy != null)
            {
                enemy.TakeDamage(_selectedStars.Count*10);
                print("Inflicted: " + _selectedStars.Count*10);
                foreach (var star in _selectedStars)
                {
                    star.Deselect();
                }
                _selectedStars.Clear();
            }
        }
        else 
        {
            foreach (Star star in _selectedStars)
            {
                star.Deselect();
            }
            _selectedStars.Clear();
        }
    }

    private void FixedUpdate()
    {
        if (_locked)
        {
            TargetStar();
        }

        if (_attack)
        {
            _attack = false;
            PerformAttack();
        }
    }
}
