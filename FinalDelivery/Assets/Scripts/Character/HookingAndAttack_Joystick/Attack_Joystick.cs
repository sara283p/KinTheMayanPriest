﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack_Joystick : MonoBehaviour
{
    public Locker_Joystick locker;

    public LineRenderer lineRenderer;
    public LayerMask obstacleLayerMask;
    public Sprite normalViewFinder;
    public Sprite killingViewFinder;
    
    private float maxAllowedDistance; // Max distance at which the first star selected can be
    public float thresholdManualAttackDistance;

    private enum TargetType {Star, Enemy}

    private TargetType _targetType = TargetType.Star;

    private bool _attacking;
    private bool _isAttackOngoing;
    private bool _selecting;
    private bool _readyToSelect = true;

    private const float UpThresholdSelect = 0.5f;
    private const float DownThresholdSelect = 0.2f;
    private float _analogDeadZone;

    private Star _targetStar;
    private IDamageable _targetEnemy;
    private Transform _tr;
    
    private readonly List<Star> _selectedStars = new List<Star>();

    public MoveStarViewfinder_Joystick viewfinder;

    private AudioManager _audioManager;

    public bool isHanging;
    private bool _autoTarget = true;
    private Animator _redSphereAnimator;
    private Transform _redSphere;
    private int _maxLinkableStars;
    private float _attackBonus;
    private SpriteRenderer _viewFinderSpriteRenderer;
    private float _bonusDamage;
    private float _baseDamage;
    
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

    private void Awake()
    // Initialize the attack effect
    {
        _audioManager = AudioManager.instance;
        lineRenderer.positionCount = 0;
        maxAllowedDistance = GameManager.Instance.maxStarSelectDistance;
        _tr = GetComponent<Transform>();
        maxAllowedDistance = GameManager.Instance.maxStarSelectDistance;
        _redSphereAnimator = gameObject.GetComponentInChildren<AttackRedSphere>().GetComponent<Animator>();
        _redSphere = gameObject.GetComponentInChildren<AttackRedSphere>().transform;
        _maxLinkableStars = GameManager.Instance.linkableStars;
        _analogDeadZone = GameManager.Instance.analogDeadZone;
        _attackBonus = GameManager.Instance.attackBonus;
        _baseDamage = 0;
        _bonusDamage = 0;
        _redSphere.gameObject.SetActive(false);
        _viewFinderSpriteRenderer = viewfinder.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("LinkableStarsIncreased", IncreaseLinkableStars);
    }

    private void OnDisable()
    {
        EventManager.StopListening("LinkableStarsIncreased", IncreaseLinkableStars);
        StopAllCoroutines();
        lineRenderer.positionCount = 0;
    }

    private void IncreaseLinkableStars()
    {
        _maxLinkableStars++;
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
        if (_selectedStars.Count > 0)
        {
            _redSphere.gameObject.SetActive(true);
            _redSphereAnimator.SetBool(IsAttacking, true);
        }
        else
        {
            _redSphere.gameObject.SetActive(false);
            _redSphereAnimator.SetBool(IsAttacking, false);
        }

        // If hanging, just freeze the attack situation
        if (isHanging) return;
        
        // Move viewfinder when not doing anything
        if (_autoTarget)
        {
            var target = locker.GetNearestAvailableStar(maxAllowedDistance);
            if (target)
            {
                viewfinder.DisplayViewfinder(true);
                viewfinder.gameObject.transform.position = target.transform.position;
            }
            else viewfinder.DisplayViewfinder(false);
        }
        else
        {
            if(_targetStar)
                viewfinder.DisplayViewfinder(true);
            Vector2 relativePosition = viewfinder.transform.position - _tr.position;
            // If the player entered in manual target mode and then moves away, come back in autotarget mode
            if (relativePosition.magnitude > thresholdManualAttackDistance && _selectedStars.Count == 0)
            {
                lineRenderer.positionCount = 0;
                Abort();
                _autoTarget = true;
            }
        }
        
        // Move stars and effects if the sky is rotating
        // if (InputManager.GetButton("Button3"))
        // {
        //     if (_targetStar) viewfinder.gameObject.transform.position = _targetStar.transform.position;
        //     var positions = _selectedStars.Select(x => x.transform.position).ToList();
        //     positions.Insert(0, _blueSphere.position);
        //     lineRenderer.SetPositions(positions.ToArray());
        // }

        // If the player moves the viewfinder or selects a star or press select button, go in attack mode
        IsSelectPressed();
        if ((Math.Abs(InputManager.GetAxisRaw("RHorizontal")) > _analogDeadZone 
             || Math.Abs(InputManager.GetAxisRaw("RVertical")) > _analogDeadZone || _selecting) && !_attacking)
        {
            _autoTarget = false;
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

        // If the player press LB/RB, change the target type 
        if ((InputManager.GetButtonDown("LB") || InputManager.GetButtonDown("RB")) && _attacking)
        {
            if (_targetType == TargetType.Star)
            {
                var pointedEnemy = locker.GetNearestAvailableEnemy(_selectedStars.Select(x => x.transform.position).LastOrDefault(), maxAllowedDistance);
                if (pointedEnemy != null)
                {
                    EventManager.TriggerEvent("TargetAcquired");
                    _targetStar = _selectedStars.LastOrDefault();
                    _targetType = TargetType.Enemy;
                    _targetEnemy = pointedEnemy;
                    viewfinder.gameObject.transform.position = pointedEnemy.GetPosition();
                }
                else
                {
                    viewfinder.EnemyTooFarEffect();
                }
                
            }
            else
            {
                _targetType = TargetType.Star;
                viewfinder.gameObject.transform.position = _selectedStars.Select(x => x.transform.position).LastOrDefault();
            }
        }

        // Reset the attack
        if (InputManager.GetButtonDown("Button1"))
        {
            lineRenderer.positionCount = 0;
            Abort();
        }

        // Check:
        // 1) Is the first star gone too far?
        // 2) An obstacle is now between the first star and Kin?
        // 3) Is there any targeted stars which is now not usable?
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
            RaycastHit2D hit = Physics2D.Raycast(_tr.position, relativePosition, relativePosition.magnitude,
                obstacleLayerMask);
            if (hit.collider)
            {
                lineRenderer.positionCount = 0;
                Abort();
                return;
            }

            for (int i = 0; i < _selectedStars.Count - 1; i++)
            {
                relativePosition = _selectedStars[i + 1].transform.position - _selectedStars[i].transform.position;
                if (Physics2D.Raycast(_selectedStars[i].transform.position, relativePosition,
                    relativePosition.magnitude,
                    obstacleLayerMask))
                {
                    lineRenderer.positionCount = 0;
                    Abort();
                    return;
                }

            }

            var starsWentUnavailable = _selectedStars.Where(x => x.isDisabled).ToList();
            if (starsWentUnavailable.Count > 0)
            {
                if (_targetType == TargetType.Star)
                {
                    // Reset the viewfinder on the last targeted star. It's quite arbitrary, it may not been the best
                    // solution.
                    viewfinder.gameObject.transform.position =
                        _selectedStars.Select(x => x.transform.position).LastOrDefault();
                }
                
                starsWentUnavailable.ForEach(x =>
                {
                    x.DeselectForAttack();
                    _selectedStars.Remove(x);
                    lineRenderer.positionCount--;
                    var positions = _selectedStars.Select(y => x.transform.position).ToList();
                    positions.Insert(0, _redSphere.position);
                    lineRenderer.SetPositions(positions.ToArray());
                });
                
            }
        }
        
        // If the enemy is moving and it's targetted, move the viewfinder with him
        if (_attacking && _targetType == TargetType.Enemy && _targetEnemy != null)
        {
            viewfinder.gameObject.transform.position = _targetEnemy.GetPosition();
            Vector2 relativePosition = (Vector2) _selectedStars.Last().transform.position - _targetEnemy.GetPosition();
            RaycastHit2D hit = Physics2D.Raycast(_targetEnemy.GetPosition(), relativePosition,
                relativePosition.magnitude, obstacleLayerMask);
            if (hit.collider)
            {
                lineRenderer.positionCount = 0;
                Abort();
                return;
            }
        }

        if (_targetType == TargetType.Star)
        {
            _viewFinderSpriteRenderer.sprite = normalViewFinder;
        }
        
        if (_attacking)
        {
            // Update the effect first position
            lineRenderer.SetPosition(0, (Vector2) _redSphere.position);
            
            if (_targetType == TargetType.Star)
            {
                TargetStar();
            }
            
            if (_targetType == TargetType.Enemy && _selectedStars.Count > 0)
            {
                TargetEnemy();
                _viewFinderSpriteRenderer.sprite = _targetEnemy.IsEnemy() && _baseDamage + _bonusDamage >= ((Health) _targetEnemy).GetHealth() ? killingViewFinder : normalViewFinder;
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
        if (InputManager.GetAxisRaw("RTrigger") > _analogDeadZone && _readyToSelect)
        {
            _selecting = true;
            _readyToSelect = false;
        } 
        if (InputManager.GetAxisRaw("RTrigger") <= _analogDeadZone)
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
                var lastStar = _selectedStars.LastOrDefault();
                if (lastStar)
                {
                    Vector2 relativePosition = _targetStar.transform.position - lastStar.transform.position;
                    if (Physics2D.Raycast(lastStar.transform.position,
                        relativePosition, relativePosition.magnitude,
                        obstacleLayerMask))
                    {
                        return;
                    }
                }

                if (_selectedStars.Count >= _maxLinkableStars)
                {
                    return;
                }
                _targetStar.SelectForAttack();
                _selectedStars.Add(_targetStar);
                _baseDamage += _targetStar.damagePoints;
                
                // Attack bonus in case all linkable stars are selected
                if (_selectedStars.Count > 1 && _selectedStars.Count == _maxLinkableStars)
                {
                    _bonusDamage = _attackBonus * _selectedStars.First().damagePoints;
                }
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(_selectedStars.Count, (Vector2) _targetStar.transform.position);
            }
            else
            {
                _targetStar.DeselectForAttack();
                _bonusDamage = 0;
                _selectedStars.Remove(_targetStar);
                _baseDamage -= _targetStar.damagePoints;
                lineRenderer.positionCount--;
                var positions = _selectedStars.Select(x => x.transform.position).ToList();
                positions.Insert(0, _redSphere.position);
                lineRenderer.SetPositions(positions.ToArray());
            }
        }
    }

    private void Abort()
    {
        _baseDamage = _bonusDamage = 0;
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
        {
            // A star is already targeted. If the first star has not already been selected, let the user choose it.
            // else, let the user choose the next available star
            if (_selectedStars.Count == 0)
            {
                var pointedStar = locker.GetFirstStarByRaycast(viewfinder.transform, maxAllowedDistance, _tr);
                if (pointedStar)
                {
                    _targetStar = pointedStar;
                    viewfinder.gameObject.transform.position = pointedStar.transform.position;
                }

            }
            else
            {
                var pointedStar = locker.GetAvailableStarByRaycast(viewfinder.transform, maxAllowedDistance);
                if (pointedStar)
                {
                    _targetStar = pointedStar;
                    viewfinder.gameObject.transform.position = pointedStar.transform.position;
                }
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
            if (_targetEnemy == null)
            {
                viewfinder.EnemyTooFarEffect();
                return;
            }
            viewfinder.gameObject.transform.position = _targetEnemy.GetPosition();
        }
        else
        {
            var pointedEnemy = locker.GetAvailableEnemyByRaycast(_targetEnemy.GetTransform(),
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
            var damage = _baseDamage + _bonusDamage;
            _selectedStars.ForEach(star => star.UseForAttack());
            _targetEnemy.TakeDamage(damage);
            print("Inflicted: " + damage);
            _audioManager.Play("Attack");
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
