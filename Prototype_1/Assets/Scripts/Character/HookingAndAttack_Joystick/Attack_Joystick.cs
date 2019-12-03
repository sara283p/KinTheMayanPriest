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
    
    public float maxAllowedDistance; // Max distance at which the first star selected can be

    private enum TargetType {Star, Enemy}

    private TargetType _targetType = TargetType.Star;

    private bool _attacking;
    private bool _isAttackOngoing;
    private bool _selecting;
    private bool _readyToSelect = true;

    private const float UpThresholdSelect = 0.5f;
    private const float DownThresholdSelect = 0.2f;
    private const float ThresholdViewfinder = 0.3f;

    private Star _targetStar;
    private IDamageable _targetEnemy;
    private Transform _tr;
    
    private readonly List<Star> _selectedStars = new List<Star>();

    public MoveStarViewfinder_Joystick viewfinder;

    public bool isHanging;
    private bool _autoTarget = true;

    private void Awake()
    // Initialize the attack effect
    {
        lineRenderer.positionCount = 0;
        _tr = GetComponent<Transform>();
        maxAllowedDistance = GameManager.Instance.maxStarSelectDistance;
    }

    public void SetHanging(bool val)
    {
        if (val)
        {
            isHanging = true;
            lineRenderer.positionCount = 0;
            Abort();
        }
        else isHanging = false;
    }

    public void AutoTargetWorking(bool val)
    {
        if(!val) _autoTarget = false;
    }

    void Update()
    {
        if (isHanging) return;
        // Move viewfinder when not doing anything
        if (_autoTarget)
        {
            var target = locker.GetNearestAvailableStar();
            if (target)
            {
                viewfinder.DisplayViewfinder(true);
                viewfinder.gameObject.transform.position = target.transform.position;
            }
            else viewfinder.DisplayViewfinder(false);
        }
        
        // Move stars and effects if the sky is rotating
        if (InputManager.GetButton("Button2"))
        {
            if (_targetStar) viewfinder.gameObject.transform.position = _targetStar.transform.position;
            var positions = _selectedStars.Select(x => x.transform.position).ToList();
            positions.Insert(0, _tr.position);
            lineRenderer.SetPositions(positions.ToArray());
        }

        // If the player moves the viewfinder or selects a star or press select button, go in attack mode
        IsSelectPressed();
        if ((InputManager.GetAxisRaw("RHorizontal") > ThresholdViewfinder 
             || InputManager.GetAxisRaw("RVertical") > ThresholdViewfinder || _selecting) && !_attacking)
        {
            _autoTarget = false;
            _selecting = false;
            if (!locker.GetNearestAvailableStar(maxAllowedDistance)) return;
            if (_isAttackOngoing)
            {
                _isAttackOngoing = false;
                lineRenderer.positionCount = 0;
            }
            _attacking = true;
            _targetType = TargetType.Star;
            lineRenderer.positionCount++;
        }

        if ((InputManager.GetButtonDown("LB") || InputManager.GetButtonDown("RB")) && _attacking)
        {
            if (_targetType == TargetType.Star)
            {
                var pointedEnemy = locker.GetNearestAvailableEnemy(_selectedStars.Select(x => x.transform.position).LastOrDefault(), maxAllowedDistance);
                if (pointedEnemy != null)
                {
                    _targetType = TargetType.Enemy;
                    _targetEnemy = pointedEnemy;
                    viewfinder.gameObject.transform.position = pointedEnemy.GetPosition();
                }
                
            }
            else
            {
                _targetType = TargetType.Star;
                viewfinder.gameObject.transform.position = _selectedStars.Select(x => x.transform.position).LastOrDefault();
            }
        }

        if (InputManager.GetButtonDown("Button1"))
        {
            lineRenderer.positionCount = 0;
            Abort();
        }

        if (_attacking && _selectedStars.Count > 0)
        {
            Vector2 relativePosition = _selectedStars.First().transform.position - _tr.position;

            if (relativePosition.magnitude > maxAllowedDistance)
            {
                lineRenderer.positionCount = 0;
                Abort();
                return;
            }
            
            Debug.DrawRay(_tr.position, relativePosition, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(_tr.position, relativePosition, relativePosition.magnitude, obstacleLayerMask);
            if (hit.collider)
            {
                lineRenderer.positionCount = 0;
                Abort();
            }
        }
        
        if (_attacking && _targetType == TargetType.Enemy && _targetEnemy != null)
        {
            viewfinder.gameObject.transform.position = _targetEnemy.GetPosition();
        }
        
        if (_attacking)
        {
            lineRenderer.SetPosition(0, _tr.position);
            
            if (_targetType == TargetType.Star)
            {
                TargetStar();
            }
            
            if (_targetType == TargetType.Enemy && _selectedStars.Count > 0)
            {
                TargetEnemy();
            }

            // If the Select button is pressed, there are three possibilities:
            // 1) The player wants to select a star for attack
            // 2) The player wants to deselect a star for attack
            // 3) The player wants to cast an attack to an enemy
            IsSelectPressed();
            if (_selecting)
            {
                _selecting = false;
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

    private void IsSelectPressed()
    {
        if (InputManager.GetAxisRaw("LTrigger") > UpThresholdSelect && _readyToSelect)
        {
            _selecting = true;
            _readyToSelect = false;
        } 
        if (InputManager.GetAxisRaw("LTrigger") <= DownThresholdSelect)
        {
            _readyToSelect = true;
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
                lineRenderer.SetPosition(_selectedStars.Count, _targetStar.transform.position);
            }
            else
            {
                _targetStar.DeselectForAttack();
                _selectedStars.Remove(_targetStar);
                lineRenderer.positionCount--;
                var positions = _selectedStars.Select(x => x.transform.position).ToList();
                positions.Insert(0, _tr.position);
                lineRenderer.SetPositions(positions.ToArray());
            }
        }
    }

    private void Abort()
    {
        _autoTarget = true;
        _attacking = false;
        _selecting = false;
        _targetStar = null;
        _targetEnemy = null;
        _targetType = TargetType.Star;

        _selectedStars.ForEach(x => x.DeselectForAttack());
        _selectedStars.Clear();
        
    }

    private void TargetStar()
    {
        // Then branch: the user has just entered Star target mode.
        // Just move to the nearest available star.
        if (!_targetStar)
        {
            _targetStar = locker.GetNearestAvailableStar(maxAllowedDistance);
            if (!_targetStar) return;
            viewfinder.gameObject.transform.position = _targetStar.transform.position;
        }
        else
        // The first star has already been choosen: try to understand if the user is targeting a star or an enemy
        // and act accordingly.
        {
            var pointedStar = locker.GetAvailableStarByRaycast(viewfinder.transform, maxAllowedDistance);
            if (pointedStar)
            {
                _targetStar = pointedStar;
                viewfinder.gameObject.transform.position = pointedStar.transform.position;
            }
        }
    }

    private void TargetEnemy()
    {
        // Then branch: the user has just entered Enemy target mode.
        // Just move to the nearest available enemy.
        if (_targetEnemy == null)
        {
            _targetEnemy =
                locker.GetNearestAvailableEnemy(_selectedStars.Select(x => x.transform.position).LastOrDefault(), maxAllowedDistance);
            if (_targetEnemy == null) return;
            viewfinder.gameObject.transform.position = _targetEnemy.GetPosition();
        }
        else
        {
            var pointedEnemy = locker.GetAvailableEnemyByRaycast(viewfinder.transform,
                _selectedStars.Select(x => x.transform.position).LastOrDefault(), maxAllowedDistance);
            if (pointedEnemy != null)
            {
                _targetEnemy = pointedEnemy;
                viewfinder.gameObject.transform.position = pointedEnemy.GetPosition();
            }
        }
    }

    private void PerformAttack()
    {
        if (_targetEnemy != null)
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

    IEnumerator AttackEffect(IDamageable enemy)
    {
        _isAttackOngoing = true;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(_selectedStars.Count + 1, enemy.GetPosition());

        yield return new WaitForSeconds(2f);
        
        if (_isAttackOngoing)
        {
            _isAttackOngoing = false;
            lineRenderer.positionCount = 0;
        }
    }
    
}
