using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementStats _movementStats;

    [Header("Colliders")]
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    // movement vars
    private Vector2 _moveVelocity;

    // turn vars
    private bool _isFacingRight;
    private Vector2 _mousePosition;
    private float _turnThreshold;

    // collision vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;

    // jump vars
    private float _verticalVelocity;
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    // coyote time vars
    private float _coyoteTimer;

    void Awake()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        TurnCheck();
        if (_isGrounded)
        {
            Move(_movementStats.GroundAcceleration, _movementStats.GroundDeceleration, InputManager.Movement);
        } else
        {
            Move(_movementStats.AirAcceleration, _movementStats.AirDeceleration, InputManager.Movement);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunIsHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * _movementStats.MaxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * _movementStats.MaxWalkSpeed;
            }
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        } else
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
    }

    private void TurnCheck()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _turnThreshold = transform.position.x;
        if (_isFacingRight && _mousePosition.x < _turnThreshold)
        {
            Turn(false);
        } else if (!_isFacingRight && _mousePosition.x > transform.position.x)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            transform.Rotate(0f, 180f, 0f);
            _isFacingRight = true;
        }
        else
        {
            transform.Rotate(0f, -180f, 0f);
            _isFacingRight = false;
        }
    }

    #endregion

    #region Jump

    private void JumpChecks()
    {
        // WHEN JUMP IS PRESSED
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = _movementStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // WHEN JUMP IS RELEASED
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && _verticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = _movementStats.TimeForUpwardsCancel;
                    _verticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = _verticalVelocity;
                }
            }
        }

        // INITIATE JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed =_verticalVelocity;
            }
        }
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < _movementStats.NumberOfJumpsAllowed) //DOUBLE JUMP
        {
            _isFastFalling = false;
            InitiateJump(1);
        }
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < _movementStats.NumberOfJumpsAllowed - 1) // AIR JUMP AFTER COYOTE TIME LAPSED
        {
            InitiateJump(2);
            _isFastFalling = false;
        }

        // LANDED
        if ((_isJumping || _isFalling) && _isGrounded && _verticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFastFalling = false;
            _isFalling = false;
            _fastFallTime = 0f;
            _numberOfJumpsUsed = 0;
            _isPastApexThreshold = false;

            _verticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }
        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        _verticalVelocity = _movementStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // APPLY GRAVITY
        if (_isJumping)
        {
            // CHECK FOR HEAD BUMP
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            // GRAVITY ON ASCENDING
            if (_verticalVelocity > 0)
            {
                // APEX CONTROLS
                _apexPoint = Mathf.InverseLerp(_movementStats.InitialJumpVelocity, 0f, _verticalVelocity);

                if(_apexPoint >= _movementStats.ApexThreshold)
                {
                    if(!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }
                    else
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold >= _movementStats.ApexHangTime)
                        {
                            _verticalVelocity = 0f;
                        }
                        else
                        {
                            _verticalVelocity = -0.01f;
                        }
                    }
                }
                else // GRAVITY ON ASCENDING BUT NOT PAST APEX THRESHOLD
                {
                    _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime;
                    if(_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
            else if (!_isFastFalling) // GRAVITY ON DESCENDING
            {
                _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime * _movementStats.GravityOnReleaseMultiplier; 
            }
            else if (_verticalVelocity < 0f)
            {
                if(!_isFalling)
                {
                    _isFalling = true;
                }
            }

        }

        // JUMP CUT

        if (_isFastFalling)
        {
            if (_fastFallTime > _movementStats.TimeForUpwardsCancel)
            {
                _verticalVelocity += _movementStats.Gravity * _movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < _movementStats.TimeForUpwardsCancel)
            {
                _verticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, _fastFallTime / _movementStats.TimeForUpwardsCancel);

            }
            _fastFallTime += Time.fixedDeltaTime;
        }

        // NORMAL GRAVITY WHILE FALLING
        if(!_isGrounded && !_isJumping)
        {
            if(!_isFalling)
            {
                _isFalling = true;
            }
            _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime;
        }

        // CLAMP FALL SPEED
        _verticalVelocity = Mathf.Clamp(_verticalVelocity, -_movementStats.MaxFallSpeed, 50f);

        // APPLY VERTICAL VELOCITY
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _verticalVelocity);
    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, _movementStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, _movementStats.GroundDetectionRayLength, _movementStats.GroundLayer);

        if(_groundHit.collider != null)
        {
            _isGrounded = true;
        } else
        {
            _isGrounded = false;
        }
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_bodyColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_bodyColl.bounds.size.x * _movementStats.HeadWidth, _movementStats.HeadDetectionRayLength);
        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, _movementStats.HeadDetectionRayLength, _movementStats.GroundLayer);
        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = _movementStats.JumpCoyoteTime;
        }
    }

    #endregion
}
