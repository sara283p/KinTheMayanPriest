using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack_Joystick : MonoBehaviour
{
    public Locker_Joystick locker;

    public LineRenderer lineRenderer;
    public LayerMask obstacleLayerMask;

    public float selectionDelay = 0.4f;

    enum TargetType {Star, Enemy}

    private TargetType _targetType = TargetType.Star;

    private bool _attacking;
    private bool _isAttackOngoing;
    private bool _skyIsRotating;

    private bool _selectingWait;

    private Star _targetStar;
    private Star _firstStar;
    private Enemy _targetEnemy;
    private Transform _tr;
    
    private List<Star> _selectedStars = new List<Star>();

    public MoveStarViewfinder_Joystick viewfinder;

    private void Awake()
    // Initialize the attack effect
    {
        lineRenderer.positionCount = 0;
        _tr = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetButtonDown("RotateSky"))
        {
            _skyIsRotating = true;
        }
        else if (Input.GetButtonUp("RotateSky"))
        {
            _skyIsRotating = false;
        }
        
        // If the LockAttack button is kept pressed, enter in mode "Attack"
        // In this mode you can move through available (not in cooldown) stars, select them for attack,
        // deselect them if you change your mind, move to an enemy and cast the attack.
        // If, in every of these moments, the LockAttack button is released, exit from mode "Attack".
        if (Input.GetButtonDown("LockAttack"))
        {
            _attacking = true;
        }

        if (Input.GetButtonUp("LockAttack") || _skyIsRotating)
        {
            _attacking = false;
            Abort();
            if (!_isAttackOngoing) lineRenderer.positionCount = 0;
        }

        if (_firstStar)
        {
            Vector2 relativePosition = _firstStar.transform.position - _tr.position;
            Debug.DrawRay(_tr.position, relativePosition, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(_tr.position, relativePosition, relativePosition.magnitude, obstacleLayerMask);
            if (hit.collider)
            {
                Abort();
                _attacking = true;
                if (!_isAttackOngoing) lineRenderer.positionCount = 0;
                return;
            }
        }
        
        if (_attacking)
        {
            // This if is just for the animation of the attack.
            if (_isAttackOngoing)
            {
                _isAttackOngoing = false;
                lineRenderer.positionCount = 0;
            }
            
            Target();
            
            // If the Select button is pressed, there are three possibilities:
            // 1) The player wants to select a star for attack
            // 2) The player wants to deselect a star for attack
            // 3) The player wants to cast an attack to an enemy
            if (Input.GetButtonDown("Select"))
            {
                if (_targetType == TargetType.Star)
                {
                    SelectStar();
                }
            
                if (_targetType == TargetType.Enemy && _selectedStars.Count > 0)
                {
                    PerformAttack();
                }
            }
        }
    }

    private void SelectStar()
    {
        if (_targetStar) 
        {
            // Either select a star, so put it in the _selectedStars, activate it and add the position to animation
            // or deselect a star already selected, so remove it, deselect it, and redraw the animation.
            if (!_targetStar.IsSelectedForAttack())
            {
                _targetStar.SelectForAttack();
                _selectedStars.Add(_targetStar);
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(_selectedStars.Count - 1, _targetStar.transform.position);
            }
            else
            {
                _targetStar.DeselectForAttack();
                _selectedStars.Remove(_targetStar);
                lineRenderer.positionCount--;
                lineRenderer.SetPositions(_selectedStars.Select( x => x.transform.position).ToArray());
            }
        }
    }

    private void Abort()
    {
        _attacking = false;
        _targetStar = null;
        _firstStar = null;
        _targetEnemy = null;

        _selectedStars.ForEach(x => x.DeselectForAttack());
        _selectedStars.Clear();
        
        viewfinder.DisplayViewfinder(false);
    }

    private void Target()
    {
        // Then branch: the user has just entered Attack mode.
        // Just move to the nearest available star.
        if (!_targetStar)
        {
            _targetStar = locker.GetNearestAvailableStar();
            _firstStar = _targetStar;
            if (!_targetStar) return;
            viewfinder.DisplayViewfinder(true);
            viewfinder.gameObject.transform.position = _targetStar.transform.position;
            _targetType = TargetType.Star;
        }
        else
        // The first star has already been choosen: try to understand if the user is targeting a star or an enemy
        // and act accordingly.
        {
            var pointedStar = locker.GetAvailableStarByRaycast(viewfinder.transform);
            if (pointedStar)
            {
                _targetType = TargetType.Star;
                _targetStar = pointedStar;
                viewfinder.gameObject.transform.position = pointedStar.transform.position;
            }
            else
            {
                var pointedEnemy = locker.GetEnemyByRaycast();
                if (pointedEnemy)
                {
                    _targetType = TargetType.Enemy;
                    _targetEnemy = pointedEnemy;
                    viewfinder.gameObject.transform.position = pointedEnemy.transform.position;
                }
            }
        }
    }

    private void PerformAttack()
    {
        if (_targetEnemy)
        {
            StartCoroutine(AttackEffect(_targetEnemy));
            var damage = _selectedStars.Select(x => x.damagePoints).Sum();
            _selectedStars.ForEach(star => star.UseForAttack());
            
            _targetEnemy.TakeDamage(damage);
            print("Inflicted: " + damage);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
        
        Abort();
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
    
}
