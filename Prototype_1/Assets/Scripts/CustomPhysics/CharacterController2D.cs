using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics
{
    public class CharacterController2D : MonoBehaviour
    {
        public float maxHorizontalSpeed;
        public float maxJumpSpeed;
        public float mass;

        private Vector2 _currentVelocity;
        private bool _isJumping;
        
        
        public float shellRadius;
        public float minGroundNormal;
    
        private Rigidbody2D _rb;
        public bool _grounded;
        private ContactFilter2D _contactFilter;
        private RaycastHit2D[] _raycastBuffer;
        private Vector2 _groundNormal;
        private float _minMoveDistance;
        private List<RaycastHit2D> _raycastList = new List<RaycastHit2D>(16);

        private void ComputeVelocity()
        {
            float x = maxHorizontalSpeed * Input.GetAxis("Horizontal");
            float y = _currentVelocity.y;
            if (Input.GetButtonDown("Jump") && _grounded)
            {
                mass = 1;
                _isJumping = true;
                y = maxJumpSpeed;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                if (_currentVelocity.y > 0)
                {
                    y = _currentVelocity.y * 0.5f;
                }
            }

            Vector2 velocity = new Vector2(x, y);
            //_currentVelocity = velocity + mass * Time.deltaTime * Physics2D.gravity;
            _currentVelocity = velocity;
        }
        
        private void Awake()
        {
            _currentVelocity = Vector2.zero;
            
            _rb = GetComponent<Rigidbody2D>();
            _minMoveDistance = 0.001f;
        }

        // Start is called before the first frame update
        void Start()
        {
            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            _contactFilter.useLayerMask = true;
            _raycastBuffer = new RaycastHit2D[16];
        }

        // Update is called once per frame
        void Update()
        {
            ComputeVelocity();
        }

        private void FixedUpdate()
        {
            _currentVelocity += mass * Time.fixedDeltaTime * Physics2D.gravity;
            /*if (_cm.IsGrounded() && _currentVelocity.y < 0)
            {
                _currentVelocity.y = 0;
            }*/
            Move();
        }
        
        private void PerformMovement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;

            if (distance > _minMoveDistance)
            {
                int count = _rb.Cast(move, _contactFilter, _raycastBuffer, distance + shellRadius);

                _raycastList.Clear();

                for (int i = 0; i < count; i++)
                {
                    // Increase mass to avoid rocket jump at the end of ramps
                    mass = 3;
                    _raycastList.Add(_raycastBuffer[i]);
                }
            
                for (int i = 0; i < _raycastList.Count; i++)
                {
                    Vector2 currentNormal = _raycastList[i].normal;
                    // If the vertical component of the hit normal is higher than the minimum threshold to consider the player grounded
                    if (currentNormal.y > minGroundNormal)
                    {
                        _grounded = true;
                        _isJumping = false;

                        if (yMovement)
                        {
                            _groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    float projection = Vector2.Dot(_currentVelocity, currentNormal);
                
                    if (projection < 0)
                    {
                        _currentVelocity = _currentVelocity - projection * currentNormal;
                    }

                    if (_raycastList[i].collider.CompareTag("FixedObstacle"))
                    {
                        float modifiedDistance = _raycastList[i].distance - shellRadius;

                        distance = modifiedDistance < distance ? modifiedDistance : distance;
                    }
                }
            }

            if (!_rb.isKinematic)
                distance = 0;
            _rb.position = _rb.position + move.normalized * distance;
        }

        public void Move()
        {
            _grounded = false;
            Vector2 delta = _currentVelocity * Time.fixedDeltaTime;
        
            // moveAlongGround is the vector perpendicular to groundNormal in the same direction of the ground itself
            Vector2 moveAlongGround;
        
            if (!_isJumping)
            {
                moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);
            }
            else
            {
                moveAlongGround = Vector2.right;
            }
        
            Vector2 move = delta.x * moveAlongGround;
            PerformMovement(move, false);

            move = delta.y * Vector2.up;
            PerformMovement(move, true);
        }

        public bool IsGrounded()
        {
            return _grounded;
        }
        
        public void SwitchToKinematic()
        {
            _rb.isKinematic = true;
            _currentVelocity = _rb.velocity;
            if (!_grounded)
                _currentVelocity.y = maxJumpSpeed;
            _rb.velocity = Vector2.zero;
        }
    }
}