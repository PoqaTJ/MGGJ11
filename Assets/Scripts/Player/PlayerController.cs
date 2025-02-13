﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private InputAction _moveAction;
        private InputAction _jumpAction;
        
        [SerializeField] private Rigidbody2D _rigidbody2D;

        // ground check
        [SerializeField] private BoxCollider2D _groundCollider;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundedCheckLength = 0.1f;

        // stats - move to scriptableobject for fast swap between power ups?
        [SerializeField] private float acceleration = 3f;
        [SerializeField] private float deceleration = 5f;
        [SerializeField] private float maxSpeed = 10f;

        // jump
        [SerializeField] private float jumpVelocity = 10f;
        [SerializeField] private float jumpShort = 5f;
        
        private Vector2 _moveVelocity;
        private Vector2 _facingRight;
        private bool _grounded;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void FixedUpdate()
        {
            GroundCheck();

            Move(_moveAction.ReadValue<Vector2>().x);
        }

        private void Update()
        {
            if (_jumpAction.triggered)
            {
                Jump();
            }

            if (!_grounded && _jumpAction.WasReleasedThisFrame())
            {
                JumpCancel();
            }
        }

        private void GroundCheck()
        {
            Vector2 groundedOrigin = new Vector2(_groundCollider.bounds.center.x, _groundCollider.bounds.min.y);
            Vector2 groundedSize = new Vector2(_groundCollider.bounds.size.x, _groundedCheckLength);
            var _groundHit = Physics2D.BoxCast(groundedOrigin, groundedSize, 0f, Vector2.down, _groundedCheckLength, _groundLayer);
            _grounded = _groundHit.collider != null;
        }

        private void Move(float hMove)
        {
            float change = hMove == 0 ? deceleration : acceleration;

            Vector2 targetVelocity = new Vector2(hMove * maxSpeed, 0);
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, change * Time.fixedDeltaTime);
            _rigidbody2D.velocity = new Vector2(_moveVelocity.x, _rigidbody2D.velocity.y);
        }

        private void Jump()
        {
            if (_grounded)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpVelocity);
            }
        }

        private void JumpCancel()
        {
            if (_rigidbody2D.velocity.y > jumpShort)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpShort);
            }
        }
    }
}