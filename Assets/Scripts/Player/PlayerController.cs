﻿using System;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private InputAction _moveAction;
        private InputAction _jumpAction;
        [SerializeField] private PlayerStats _stats;
        
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _anim;

        // ground check
        [SerializeField] private BoxCollider2D _groundCollider;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundedCheckLength = 0.1f;
        
        // wall check
        [SerializeField] private BoxCollider2D _wallCollider;
        [SerializeField] private float _wallCheckLength = 0.05f;
        
        private Vector2 _moveVelocity;
        private bool _facingRight = true;
        private bool _grounded;

        private bool _doubleJumpUnlocked;
        private bool _hasDoubleJumped;

        private bool _wallJumpUnlocked;
        private bool _walled;

        private bool _canDoubleJump => _doubleJumpUnlocked && !_hasDoubleJumped;

        private static readonly int _groundedParam = Animator.StringToHash("grounded");
        private static readonly int _hMoveParam = Animator.StringToHash("hMove");
        private static readonly int _yMoveParam = Animator.StringToHash("yMove");

        private int _maxHealth = 3;
        private int _currentHealth;
        private bool _blockMove = false;
        private float _unblockMoveTime;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");

            ServiceLocator.Instance.GameManager.OnDoublejumpUnlocked += () => _doubleJumpUnlocked = true;

            ServiceLocator.Instance.GameManager.RegisterPlayer(this);
            
            Reset();
        }

        private void FixedUpdate()
        {
            if (_blockMove && Time.timeSinceLevelLoad > _unblockMoveTime)
            {
                _blockMove = false;
                _rigidbody2D.velocity = Vector2.zero;
            }
            GroundCheck();
            WallCheck();

            Move(_moveAction.ReadValue<Vector2>().x);
        }

        private void Update()
        {
            if (_blockMove)
            {
                return;
            }
            
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
            if (_grounded)
            {
                _hasDoubleJumped = false;
            }
        }

        private void WallCheck()
        {
            Vector2 walledOrigin = new Vector2(_wallCollider.bounds.center.x, _wallCollider.bounds.min.y);
            Vector2 walledSize = new Vector2(_groundCollider.bounds.size.x, _groundedCheckLength);
            var direction = _facingRight ? Vector2.right : Vector2.left;
            var _wallHit = Physics2D.BoxCast(walledOrigin, walledSize, 0f, direction, _wallCheckLength, _groundLayer);
            _walled = _wallHit.collider != null;
            if (_walled)
            {
            }
        }

        private void Move(float hMove)
        {
            if (_blockMove)
            {
                return;
            }
            float change;
            if (_grounded)
            {
                change = hMove == 0 ? _stats.Deceleration : _stats.Acceleration;

            }
            else
            {
                change = hMove == 0 ? _stats.AirDeceleration : _stats.AirAcceleration;
            }

            if (hMove < 0)
            {
                Face(false);
            }
            else if (hMove > 0)
            {
                Face(true);
            }

            Vector2 targetVelocity = new Vector2(hMove * _stats.MaxSpeed, 0);
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, change * Time.fixedDeltaTime);
            _rigidbody2D.velocity = new Vector2(_moveVelocity.x, _rigidbody2D.velocity.y);
            
            _anim.SetBool(_groundedParam, _grounded);
            _anim.SetFloat(_hMoveParam, Mathf.Abs(hMove));
            _anim.SetFloat(_yMoveParam, _rigidbody2D.velocity.y);
        }

        private void Jump()
        {
            bool jump = false;
            if (_grounded)
            {
                jump = true;
            }
            else if (_canDoubleJump)
            {
                _hasDoubleJumped = true;
                jump = true;
            }

            if (jump)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _stats.JumpVelocity);
            }
        }

        private void JumpCancel()
        {
            if (_rigidbody2D.velocity.y > _stats.JumpShort)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _stats.JumpShort);
            }
        }

        private void Face(bool right)
        {
            bool rotate = false;
            if (right && !_facingRight)
            {
                _facingRight = true;
                rotate = true;
            }
            else if (!right && _facingRight)
            {
                _facingRight = false;
                rotate = true;
            }

            if (rotate)
            {
                transform.Rotate(0, 180f, 0);
            }
        }

        public void TakeDamage(int dmg, Vector2 location)
        {
            if (_blockMove)
            {
                return;
            }
            
            _currentHealth -= dmg;
            _currentHealth = Mathf.Max(0, _currentHealth);

            if (_currentHealth == 0)
            {
                Die();
            }
            else
            {
                float xdir = location.x < transform.position.x ? 1 : -1;
                _rigidbody2D.velocity = new Vector2(xdir, 1) * 5;
                _blockMove = true;
                _unblockMoveTime = Time.timeSinceLevelLoad + 0.2f;
                ServiceLocator.Instance.GameManager.OnPlayerTakeDamage?.Invoke();
            }
        }

        public void Die()
        {
            ServiceLocator.Instance.GameManager.OnPlayerDied?.Invoke();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            _currentHealth = _maxHealth;
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}