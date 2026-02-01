using UnityEngine;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BossMovement : MonoBehaviour
{
    public Rigidbody2D Rb => _rb;
    public BossMovementStats MovementStats => _movementStats;
    public bool IsFacingRight => _isFacingRight;
    public bool IsGrounded => _isGrounded;
    public Transform Target { get => _target; set => _target = value; }
    
    /// <summary>
    /// When true, BossMovement won't override velocity (for dashes/lunges)
    /// </summary>
    public bool ExternalVelocityControl { get; set; }

    [Header("Stats")]
    [SerializeField] private BossMovementStats _movementStats;

    [Header("Colliders")]
    [SerializeField] private Collider2D _feetColl;

    private Rigidbody2D _rb;
    private Animator _animator;
    private BossCombat _bossCombat;

    private Vector2 _moveVelocity;
    private float _verticalVelocity;
    private bool _isFacingRight = true;
    private bool _isGrounded;
    private RaycastHit2D _groundHit;
    private Transform _target;

    private BossMovementState _currentMovementState;
    private BossMovementIdleState _idleState;
    private BossMovementWalkState _walkState;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _bossCombat = GetComponent<BossCombat>();
        
        // Disable root motion - animations shouldn't move the boss
        if (_animator != null)
            _animator.applyRootMotion = false;
    }

    private void Start()
    {
        _idleState = new BossMovementIdleState(this);
        _walkState = new BossMovementWalkState(this);
        _currentMovementState = _idleState;
        _currentMovementState.Enter();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        
        // Skip velocity control if external system (dash coroutine) is controlling it
        if (ExternalVelocityControl)
        {
            return;
        }
        
        ApplyGravity();

        // Only run movement when combat allows (when in combat Idle, walk toward target)
        if (_bossCombat != null && _bossCombat.CurrentCombatState is not BossCombatIdleState)
        {
            SetHorizontalVelocity(0f);
            _currentMovementState = _idleState;
        }
        else
        {
            if (_currentMovementState != _walkState)
                ChangeMovementState(_walkState);
            _currentMovementState.FixedUpdate();
        }

        // Apply both horizontal and vertical velocity
        _rb.linearVelocity = new Vector2(_moveVelocity.x, _verticalVelocity);
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _verticalVelocity <= 0f)
        {
            // Grounded - stop falling
            _verticalVelocity = 0f;
        }
        else
        {
            // Apply gravity
            _verticalVelocity += _movementStats.Gravity * Time.fixedDeltaTime;
            _verticalVelocity = Mathf.Max(_verticalVelocity, -_movementStats.MaxFallSpeed);
        }
    }

    private void LateUpdate()
    {
        if (_animator != null && _movementStats != null && _movementStats.MaxWalkSpeed > 0f)
        {
            _animator.SetFloat("VerticalVelocity", _rb.linearVelocity.y);
            // Normalize speed to 0-1 range for blend tree
            float normalizedSpeed = Mathf.Abs(_moveVelocity.x) / _movementStats.MaxWalkSpeed;
            _animator.SetFloat("Speed", normalizedSpeed);
        }
    }

    public void ChangeMovementState(BossMovementState state)
    {
        if (_currentMovementState == state) return;
        _currentMovementState.Exit();
        _currentMovementState = state;
        _currentMovementState.Enter();
    }

    public void SetHorizontalVelocity(float x)
    {
        _moveVelocity.x = x;
    }

    /// <summary>
    /// Reset vertical velocity (for when boss lands or needs to stop falling)
    /// </summary>
    public void ResetVerticalVelocity()
    {
        _verticalVelocity = 0f;
    }

    public void MoveTowardTarget()
    {
        // Don't reposition during any combat action (attacks, dashes, parry, etc.)
        if (_bossCombat != null && _bossCombat.CurrentCombatState is not BossCombatIdleState)
        {
            SetHorizontalVelocity(0f);
            return;
        }
        
        if (_target == null)
        {
            SetHorizontalVelocity(0f);
            return;
        }

        // Stop moving during recharge and post-animation delay
        if (_bossCombat != null && (_bossCombat.IsRecharging || _bossCombat.IsPostAnimation))
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, _movementStats.GroundDeceleration * Time.fixedDeltaTime);
            return;
        }

        float distance = DistanceToTarget();
        float dirToTarget = _target.position.x > transform.position.x ? 1f : -1f;
        
        // Too close - back away from player
        if (distance < _movementStats.MinimumDistance)
        {
            Turn(dirToTarget > 0); // Still face the player
            float awayDir = -dirToTarget; // Move opposite direction
            Vector2 targetVelocity = new Vector2(awayDir * _movementStats.MaxWalkSpeed * 0.7f, 0f);
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, _movementStats.GroundAcceleration * Time.fixedDeltaTime);
            return;
        }
        
        // Within ideal range - stop
        if (distance <= _movementStats.StoppingDistance)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, _movementStats.GroundDeceleration * Time.fixedDeltaTime);
            return;
        }

        // Too far - move toward player
        Turn(dirToTarget > 0);
        Vector2 approachVelocity = new Vector2(dirToTarget * _movementStats.MaxWalkSpeed, 0f);
        _moveVelocity = Vector2.Lerp(_moveVelocity, approachVelocity, _movementStats.GroundAcceleration * Time.fixedDeltaTime);
    }

    public void Turn(bool turnRight)
    {
        if (_isFacingRight == turnRight) return;
        
        // Don't turn during attacks or dashes
        if (_bossCombat != null && _bossCombat.CurrentCombatState is not BossCombatIdleState)
            return;
        
        transform.localRotation = turnRight ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        _isFacingRight = turnRight;
    }

    public float DistanceToTarget()
    {
        if (_target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, _target.position);
    }

    public bool IsTargetToRight()
    {
        if (_target == null) return _isFacingRight;
        return _target.position.x > transform.position.x;
    }

    private void CollisionChecks()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (_feetColl == null) { _isGrounded = false; return; }

        Vector2 origin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 size = new Vector2(_feetColl.bounds.size.x, _movementStats.GroundDetectionRayLength);
        _groundHit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, _movementStats.GroundDetectionRayLength, _movementStats.GroundLayer);
        _isGrounded = _groundHit.collider != null;
    }
}
