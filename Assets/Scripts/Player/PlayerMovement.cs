using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerCombat))]
public class PlayerMovement : MonoBehaviour
{
    #region Public Properties
    public Rigidbody2D Rb => _rb;
    public PlayerMovementStats MovementStats => _movementStats;
    public bool IsFacingRight => _isFacingRight;
    public bool IsGrounded => _isGrounded;
    #endregion

    #region Serialized Fields
    [Header("Stats")]
    [SerializeField] private TimelineController _timelineController;
    [SerializeField] private PlayerMovementStats _movementStats;

    [Header("Colliders")]
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;
    #endregion

    #region Components
    private Rigidbody2D _rb;
    private Animator _animator;
    private PlayerCombat _playerCombat;
    #endregion

    #region Movement
    private Vector2 _moveVelocity;
    private bool _isFacingRight = true;
    #endregion

    #region Jump & Gravity
    private float _verticalVelocity;
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private int _numberOfJumpsUsed;

    private float _apexPoint;
    private bool _isPastApexThreshold;
    private float _timePastApexThreshold;

    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;
    private float _coyoteTimer;

    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    #endregion

    #region Collision
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;
    #endregion
    
    private PlayerManager _playerManager => ServiceLocator.Instance.PlayerManager;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        CountTimers();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        
        bool inCombat = _playerCombat.PlayerCombatState is not PlayerIdleState;
        
        if (inCombat)
        {
            // Buffer movement intent during combat - keep tracking target velocity
            // so movement is responsive when combat ends
            BufferMovement();
            return;
        }

        ApplyJump();
        HandleTurn();

        // Horizontal movement
        if (_isGrounded)
            Move(_movementStats.GroundAcceleration, _movementStats.GroundDeceleration, InputManager.Movement);
        else
            Move(_movementStats.AirAcceleration, _movementStats.AirDeceleration, InputManager.Movement);
    }

    private void LateUpdate()
    {
        // Set vertical velocity for animations
        _animator.SetFloat("VerticalVelocity", _rb.linearVelocity.y);

        // Use apex threshold to trigger falling animation
        bool isActuallyFalling = _isPastApexThreshold && _verticalVelocity <= 0f && !_isGrounded;
        _animator.SetBool("IsFalling", isActuallyFalling);

        // Running animation
        _animator.SetBool("IsRunning", Mathf.Abs(_moveVelocity.x) >= 0.05f);
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        float maxSpeed = _isGrounded ? _movementStats.MaxWalkSpeed : _movementStats.MaxAirSpeed;
        
        if (moveInput != Vector2.zero)
        {
            _playerManager.OnMove?.Invoke();
            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * maxSpeed;
            
            // Exponential smoothing - responsive but still smooth (frame-rate independent)
            float t = 1f - Mathf.Exp(-acceleration * Time.fixedDeltaTime);
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, t);
        }
        else
        {
            // Exponential smoothing for deceleration
            float t = 1f - Mathf.Exp(-deceleration * Time.fixedDeltaTime);
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, t);
            
            if (Mathf.Abs(_moveVelocity.x) <= 0.05f) _moveVelocity.x = 0f;
        }

        _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Continues tracking movement intent during combat states.
    /// Updates _moveVelocity at reduced speed, so movement is responsive but not jarring when combat ends.
    /// </summary>
    private void BufferMovement()
    {
        Vector2 moveInput = InputManager.Movement;
        // Buffer at 35% of max speed - enough to feel responsive, not enough to feel like sliding
        float maxSpeed = (_isGrounded ? _movementStats.MaxWalkSpeed : _movementStats.MaxAirSpeed) * 0.35f;
        float deceleration = _isGrounded ? _movementStats.GroundDeceleration : _movementStats.AirDeceleration;
        
        if (moveInput != Vector2.zero)
        {
            // Quickly snap to the reduced target velocity
            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * maxSpeed;
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, 0.3f);
        }
        else
        {
            // Decelerate to zero when not holding input
            float t = 1f - Mathf.Exp(-deceleration * Time.fixedDeltaTime);
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, t);
            if (Mathf.Abs(_moveVelocity.x) <= 0.05f) _moveVelocity.x = 0f;
        }
    }

    private void HandleTurn()
    {
        if (_isFacingRight && InputManager.Movement.x < 0) Turn(false);
        else if (!_isFacingRight && InputManager.Movement.x > 0) Turn(true);
    }

    private void Turn(bool turnRight)
    {
        transform.localRotation = turnRight ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        _isFacingRight = turnRight;
    }

    #endregion

    #region Jump

    private void HandleJumpInput()
    {
        // Jump pressed
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = _movementStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // Jump released
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f) _jumpReleasedDuringBuffer = true;

            if (_isJumping && _verticalVelocity > 0f)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = _verticalVelocity;
                _fastFallTime = 0f;
            }
        }

        // Initiate jump
        if (_jumpBufferTimer > 0f)
        {
            if (!_isJumping && (_isGrounded || _coyoteTimer > 0f))
            {
                StartJump(1);
                if (_jumpReleasedDuringBuffer) _isFastFalling = true;
            }
            else if (_isJumping && _numberOfJumpsUsed < _movementStats.NumberOfJumpsAllowed)
            {
                StartJump(1); // double jump
                _isFastFalling = false;
            }
            else if (_isFalling && _numberOfJumpsUsed < _movementStats.NumberOfJumpsAllowed - 1)
            {
                StartJump(2); // air jump after coyote
                _isFastFalling = false;
            }
        }

        // Landing
        if ((_isJumping || _isFalling) && _isGrounded && _verticalVelocity <= 0f)
        {
            ResetJump();
        }
    }

    private void StartJump(int jumpCount)
    {
        _isJumping = true;
        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += jumpCount;

        if (_numberOfJumpsUsed == 1)
        {
            _playerManager.OnJump?.Invoke();
            _playerCombat.RemoveAirAttackCooldown();
        }else if (_numberOfJumpsUsed == 2)
        {
            _playerManager.OnDoubleJump?.Invoke();
        }

        _animator.SetTrigger(_numberOfJumpsUsed == 1 ? "Jump" : "DoubleJump");
        _animator.SetBool("IsRunning", false);

        _verticalVelocity = _movementStats.InitialJumpVelocity;
        _isPastApexThreshold = false;
        _timePastApexThreshold = 0f;
    }

    private void ResetJump()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _numberOfJumpsUsed = 0;
        _verticalVelocity = Physics2D.gravity.y;
        _isPastApexThreshold = false;
        _fastFallTime = 0f;
    }

    private void ApplyJump()
    {
        // Ascending
        if (_isJumping)
        {
            if (_bumpedHead) _isFastFalling = true;

            if (_verticalVelocity > 0f)
            {
                _apexPoint = Mathf.InverseLerp(_movementStats.InitialJumpVelocity, 0f, _verticalVelocity);

                if (_apexPoint >= _movementStats.ApexThreshold)
                {
                    _isPastApexThreshold = true;
                    _timePastApexThreshold += Time.fixedDeltaTime;
                }

                // Normal gravity while ascending
                _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime;
            }
            else
            {
                _isFalling = true;
                _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime * _movementStats.GravityOnReleaseMultiplier;
            }
        }

        // Fast fall
        if (_isFastFalling)
        {
            if (_fastFallTime < _movementStats.TimeForUpwardsCancel)
            {
                _verticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, _fastFallTime / _movementStats.TimeForUpwardsCancel);
                _fastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                _verticalVelocity += _movementStats.Gravity * _movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }

        // Free fall (after jump finished)
        if (!_isGrounded && !_isJumping && !_isFastFalling)
        {
            _isFalling = true;
            _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime;
        }

        // Clamp
        _verticalVelocity = Mathf.Clamp(_verticalVelocity, -_movementStats.MaxFallSpeed, 50f);

        // Apply
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _verticalVelocity);
    }

    #endregion

    #region Collision

    private void CollisionChecks()
    {
        CheckGrounded();
        CheckHeadBump();
    }

    private void CheckGrounded()
    {
        Vector2 origin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 size = new Vector2(_feetColl.bounds.size.x, _movementStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, _movementStats.GroundDetectionRayLength, _movementStats.GroundLayer);

        bool wasGrounded = _isGrounded;
        _isGrounded = _groundHit.collider != null;

        if (_isGrounded && !wasGrounded) _animator.SetTrigger("Land");
    }

    private void CheckHeadBump()
    {
        Vector2 origin = new Vector2(_bodyColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 size = new Vector2(_bodyColl.bounds.size.x * _movementStats.HeadWidth, _movementStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(origin, size, 0f, Vector2.up, _movementStats.HeadDetectionRayLength, _movementStats.GroundLayer);
        _bumpedHead = _headHit.collider != null;
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
            _coyoteTimer -= Time.deltaTime;
        else
            _coyoteTimer = _movementStats.JumpCoyoteTime;
    }

    #endregion
}
