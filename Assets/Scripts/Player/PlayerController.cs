using Services;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats _stats;
        
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _anim;

        // ground check
        [SerializeField] private BoxCollider2D _groundCollider;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _platformLayer;
        [SerializeField] private float _groundedCheckLength = 0.1f;
        
        // wall check
        [SerializeField] private BoxCollider2D _wallCollider;
        [SerializeField] private float _wallCheckLength = 0.05f;

        private Vector2 _moveVelocity;
        private bool _facingRight = true;
        private bool _grounded;

        private bool _doubleJumpUnlocked => ServiceLocator.Instance.SaveManager.UnlockedDoubleJump;
        private bool _hasDoubleJumped;

        private bool _tripleJumpUnlocked => ServiceLocator.Instance.SaveManager.UnlockedTripleJump;
        private bool _hasTripleJumped;

        private bool _wallJumpUnlocked => ServiceLocator.Instance.SaveManager.UnlockedWallJump;
        private bool _walled;


        private bool _canDoubleJump => _doubleJumpUnlocked && !_hasDoubleJumped;
        private bool _canTripleJump => _tripleJumpUnlocked && !_hasTripleJumped;
        private bool _canWallJump => !_grounded && _wallJumpUnlocked && _walled;
        private bool _wallJumping => Time.timeSinceLevelLoad < _wallJumpEndTime;
        private bool _canCoyoteJump => _coyotetimer > 0 && !_grounded && !_hasDoubleJumped && !_hasTripleJumped;

        private float _coyotetimer;
        
        private float _wallJumpEndTime;
        private float _wallJumpDuration = 0.2f;

        private static readonly int _groundedParam = Animator.StringToHash("grounded");
        private static readonly int _hMoveParam = Animator.StringToHash("hMove");
        private static readonly int _yMoveParam = Animator.StringToHash("yMove");
        private static readonly int Spawn = Animator.StringToHash("Spawn");
        private static readonly int Transform = Animator.StringToHash("Transform");
        
        private int _maxHealth = 3;
        private int _currentHealth;
        private bool _blockMove = false;
        private float _unblockMoveTime;

        private bool _groundedLastFrame = false;

        private void Start()
        {
            ServiceLocator.Instance.GameManager.RegisterPlayer(this);
            
            Reset();
        }

        public void OnFixedUpdate(float xDir)
        {
            if (_blockMove && Time.timeSinceLevelLoad > _unblockMoveTime)
            {
                _blockMove = false;
                _rigidbody2D.velocity = Vector2.zero;
            }
            GroundCheck();
            WallCheck();

            Move(xDir);
        }

        public void OnUpdate(bool jumpTriggered, bool jumpReleased, float xDir)
        {
            if (_blockMove)
            {
                return;
            }
            
            if (jumpTriggered)
            {
                Jump(xDir);
            }

            if (!_grounded && jumpReleased && !_wallJumping)
            {
                JumpCancel();
            }

            if (_coyotetimer > 0)
            {
                _coyotetimer -= Time.deltaTime;
            }
        }

        private void GroundCheck()
        {
            Vector2 groundedOrigin = new Vector2(_groundCollider.bounds.center.x, _groundCollider.bounds.min.y);
            Vector2 groundedSize = new Vector2(_groundCollider.bounds.size.x, _groundedCheckLength);
            var _groundHit = Physics2D.BoxCast(groundedOrigin, groundedSize, 0f, Vector2.down, _groundedCheckLength, _groundLayer | _platformLayer);
            _grounded = _groundHit.collider != null;
            if (_grounded)
            {
                _hasDoubleJumped = false;
                _hasTripleJumped = false;
            }

            if (_groundedLastFrame && !_grounded)
            {
                _coyotetimer = _stats.CoyoteDuration;
            }
            _groundedLastFrame = _grounded;
        }

        private void WallCheck()
        {
            Vector2 walledOrigin = new Vector2(_wallCollider.bounds.center.x, _wallCollider.bounds.min.y);
            Vector2 walledSize = new Vector2(_groundCollider.bounds.size.x, _groundedCheckLength);
            var direction = _facingRight ? Vector2.right : Vector2.left;
            var _wallHit = Physics2D.BoxCast(walledOrigin, walledSize, 0f, direction, _wallCheckLength, _groundLayer);
            _walled = _wallHit.collider != null;
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

            if (_grounded)
            {
                if (hMove < 0)
                {
                    Face(false);
                }
                else if (hMove > 0)
                {
                    Face(true);
                }                
            }


            if (!_wallJumping)
            {
                Vector2 targetVelocity = new Vector2(hMove * _stats.MaxSpeed, 0);
                _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, change * Time.fixedDeltaTime);                
            }
            else
            {
                _moveVelocity = _rigidbody2D.velocity;
            }

            float yVelocity = _rigidbody2D.velocity.y;
            if (_walled)
            {
                //yVelocity = 0;
            }
            _rigidbody2D.velocity = new Vector2(_moveVelocity.x, yVelocity);
            
            _anim.SetBool(_groundedParam, _grounded);
            _anim.SetFloat(_hMoveParam, Mathf.Abs(hMove));
            _anim.SetFloat(_yMoveParam, _rigidbody2D.velocity.y);
        }

        private void Jump(float xDir)
        {
            bool jump = false;
            bool wallJump = false;
            if (_grounded)
            {
                Debug.Log("Normal jump!");
                jump = true;
            }
            else if (_canCoyoteJump)
            {
                Debug.Log("Coyote jump!");
                jump = true;
            }
            else if (_canWallJump)
            {
                Debug.Log("Wall jump!");
                jump = true;
                wallJump = true;
            }
            else if (_canDoubleJump)
            {
                Debug.Log("Double jump!");
                _hasDoubleJumped = true;
                jump = true;
            }
            else if (_canTripleJump)
            {
                Debug.Log("Triple jump!");
                _hasTripleJumped = true;
                jump = true;
            }
            
            if (jump)
            {
                if (wallJump)
                {

                    float xVelChange = _facingRight ? -_stats.WallJumpVelocityX : _stats.WallJumpVelocityX;
                    _moveVelocity = Vector2.zero;
                    _rigidbody2D.velocity = new Vector2(xVelChange, _stats.WallJumpVelocityY);
                    _wallJumpEndTime = Time.timeSinceLevelLoad + _wallJumpDuration;
                    Face(!_facingRight);
                }
                else
                {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _stats.JumpVelocity);                    
                    if (xDir != 0)
                    {
                        Face(xDir > 0);                
                    }
                }
            }
        }

        private void JumpCancel()
        {
            if (_rigidbody2D.velocity.y > _stats.JumpShort)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _stats.JumpShort);
            }
        }

        public void Face(bool right)
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
                _moveVelocity = Vector2.zero;
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
            _anim.SetTrigger(Spawn);
        }

        public void StopHorizontalMovement()
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}