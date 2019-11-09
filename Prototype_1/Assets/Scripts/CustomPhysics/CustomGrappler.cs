using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CustomPhysics
{
    public class CustomGrappler : MonoBehaviour
    {
        public HangingEffect hangingEffect;
        private CharacterController2D _controller;
        private CustomLocker _locker;
        private Rigidbody2D _rb;
        private bool _hook;
        private DistanceJoint2D _joint;
        private FrictionJoint2D _friction;

        private void Awake()
        {
            _controller = GetComponent<CharacterController2D>();
            _locker = GetComponent<CustomLocker>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void HookInput()
        {
            if (Input.GetButtonDown("LockStarHang"))
            {
                _hook = true;
            }
            else if (Input.GetButtonUp("LockStarHang"))
            {
                _hook = false;
            }
        }

        private void DestroyJoint()
        {
            _joint.connectedBody.gameObject.layer = 10;
            _controller.SwitchToKinematic();
            Destroy(_joint);
            Destroy(_friction);
            _joint = null;
        }

        // Update is called once per frame
        void Update()
        {
            HookInput();
            // If the player is hanged to a star and either he's not pressing the hook button or he's touching the ground,
            // destroy the joint
            if (_joint != null)
            {
                if (!_hook || _controller.IsGrounded())
                {
                    DestroyJoint();
                    _locker.SetHanging(false);
                    //hangingEffect.StopEffect();
                }
            }
            // If the player is locking a star, he's not touching the ground, he's pressing the hook button
            // and the joint does not exist yet, then create it
            else
            {
                if (_locker.IsLocked() && !_controller.IsGrounded() && _hook)
                {
                    _joint = gameObject.AddComponent<DistanceJoint2D>();
                    _friction = gameObject.AddComponent<FrictionJoint2D>();
                    _rb.isKinematic = false;
                    Rigidbody2D otherRb = _locker.GetTarget().GetComponent<Rigidbody2D>();
                    _joint.distance = (_rb.position - otherRb.position).magnitude;
                    _joint.connectedBody = otherRb;
                    _friction.maxForce = 1f;
                    _locker.GetTarget().layer = 0;
                    _locker.SetHanging(true);
                    //hangingEffect.StartEffect(otherRb.transform);
                }
            }
        }
    }
}
