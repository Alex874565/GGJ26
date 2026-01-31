using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BossMovement : MonoBehaviour
{
    public Rigidbody2D Rb => _rb;
    public BossMovementStats MovementStats => _movementStats;
    public bool IsFacingRight => _isFacingRight;
    public bool IsGrounded => _isGrounded;
    public Transform Target { get => _target; set => _target = value; }

    [Header("Stats")]
    [SerializeField] private BossMovementStats _movementStats;

    [Header("Colliders")]
    [SerializeField] private Collider2D _feetColl;

    private Rigidbody2D _rb;
    private Animator _animator;
    private BossCombat _bossCombat;

    private Vector2 _moveVelocity;
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
    }

    private void LateUpdate()
    {
        if (_animator != null)
        {
            _animator.SetFloat("VerticalVelocity", _rb.linearVelocity.y);
            _animator.SetBool("IsRunning", Mathf.Abs(_moveVelocity.x) >= 0.05f);
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
        _rb.linearVelocity = new Vector2(x, _rb.linearVelocity.y);
    }

    public void MoveTowardTarget()
    {
        if (_target == null)
        {
            SetHorizontalVelocity(0f);
            return;
        }

        float dir = _target.position.x > transform.position.x ? 1f : -1f;
        Turn(dir > 0);

        Vector2 targetVelocity = new Vector2(dir * _movementStats.MaxWalkSpeed, 0f);
        _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, _movementStats.GroundAcceleration * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
    }

    public void Turn(bool turnRight)
    {
        if (_isFacingRight == turnRight) return;
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
