using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossCombat : MonoBehaviour
{
    public Animator Animator => _animator;
    public BossCombatState CurrentCombatState => _combatState;
    public BossCombatStats BossCombatStats => _bossCombatStats;

    [SerializeField] private BossCombatStats _bossCombatStats;

    [Header("Colliders")]
    [SerializeField] private List<Collider2D> _comboAttackColliders;
    [SerializeField] private Collider2D _dashAttackCollider;
    [SerializeField] private Collider2D _heavyAttackCollider;

    [Header("Target (Player)")]
    [SerializeField] private Transform _playerTarget;

    [Header("Telegraph")]
    [SerializeField] private BossTelegraph _telegraph;

    private Animator _animator;
    private BossMovement _bossMovement;
    private PlayerCombat _playerCombat;

    private int CurrentAttackIndex => (_combatState is BossComboAttackState comboState) ? comboState.CurrentAttackIndex : -1;

    private BossCombatState _combatState;
    private BossCombatIdleState _idleState;
    private BossComboAttackState _comboAttackState;
    private BossHeavyAttackState _heavyAttackState;
    private BossDashState _dashState;
    private BossDashAttackState _dashAttackState;
    private BossDeadState _deadState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bossMovement = GetComponent<BossMovement>();
        
        if (_telegraph == null)
            _telegraph = GetComponent<BossTelegraph>();
    }

    private void Start()
    {
        _idleState = new BossCombatIdleState(_bossMovement, this);
        _comboAttackState = new BossComboAttackState(_bossMovement, this);
        _heavyAttackState = new BossHeavyAttackState(_bossMovement, this);
        _dashState = new BossDashState(_bossMovement, this);
        _dashAttackState = new BossDashAttackState(_bossMovement, this);
        _deadState = new BossDeadState(_bossMovement, this);

        _combatState = _idleState;
        _combatState.Enter();
    }

    public void SetPlayerTarget(Transform target)
    {
        _playerTarget = target;
        if (_bossMovement != null)
            _bossMovement.Target = target;
        
        if (target != null)
            _playerCombat = target.GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        ChangeTimers();
        CheckConditions();
        CheckStateTransitions();
        _combatState.Update();
    }

    private void FixedUpdate()
    {
        _combatState.FixedUpdate();
    }

    private void CheckConditions()
    {
        // Condition-driven "input": set buffer-like flags or cooldowns based on distance, health, etc.
        // You can add more conditions (e.g. health %, phase) here.
        float dist = _bossMovement.DistanceToTarget();
        if (_playerTarget == null) return;

        // Example: when in idle long enough, "request" attacks based on distance (handled in CheckStateTransitions)
        // No direct "buffer" like player; transitions are decided in CheckStateTransitions.
    }

    private void CheckStateTransitions()
    {
        if (_combatState is BossDeadState) return;

        float dist = _bossMovement.DistanceToTarget();
        var idleState = _idleState;

        switch (_combatState)
        {
            case BossCombatIdleState:
                if (!_bossMovement.IsGrounded) break;

                if (idleState.TimeInIdle < _bossCombatStats.MinIdleTimeBeforeAttack) break;

                // Prefer dash from range, then heavy, then combo when close
                if (dist <= _bossCombatStats.ComboAttackRange && ShouldEnterComboAttack())
                {
                    ChangeState(_comboAttackState);
                }
                else if (dist <= _bossCombatStats.HeavyAttackRange && ShouldEnterHeavyAttack())
                {
                    ChangeState(_heavyAttackState);
                }
                else if (dist <= _bossCombatStats.DashRange && ShouldEnterDash())
                {
                    ChangeState(_dashState);
                }
                else if (ShouldEnterDashAttack())
                {
                    ChangeState(_dashAttackState);
                }
                break;

            case BossDashState:
                if (ShouldEnterDashAttack())
                {
                    ChangeState(_dashAttackState);
                }
                break;
        }
    }

    private void ChangeTimers()
    {
        _comboAttackState.TimeSinceExit += Time.deltaTime;
        _heavyAttackState.TimeSinceExit += Time.deltaTime;
        _dashState.TimeSinceExit += Time.deltaTime;
    }

    private void ChangeState(BossCombatState state)
    {
        _combatState.Exit();
        _combatState = state;
        _combatState.Enter();
    }

    #region Should Enter State (Conditions)

    public bool ShouldEnterComboAttack()
    {
        return _bossMovement.IsGrounded
            && _comboAttackState.TimeSinceExit > _bossCombatStats.ComboAttackCooldown;
    }

    public bool ShouldEnterHeavyAttack()
    {
        return _bossMovement.IsGrounded
            && !_heavyAttackState.IsAttacking
            && _heavyAttackState.TimeSinceExit > _bossCombatStats.HeavyAttackCooldown;
    }

    public bool ShouldEnterDash()
    {
        return _dashState.TimeSinceExit > _dashState.Cooldown;
    }

    public bool ShouldEnterDashAttack()
    {
        return _combatState is BossDashState
            || _dashState.TimeSinceExit < _bossCombatStats.AfterDashAttackDelay;
    }

    #endregion

    #region General Combat

    private void PerformAttack(AttackData attackData, Collider2D attackCollider)
    {
        if (attackCollider == null) return;

        // Stop telegraph when attack lands
        if (_telegraph != null)
            _telegraph.StopTelegraph();

        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.SetLayerMask(LayerMask.GetMask("Player"));

        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapCollider(attackCollider, filter, hits);

        foreach (Collider2D hit in hits)
        {
            // Check if player defended correctly
            if (DidPlayerDefendCorrectly(attackData.AttackType))
            {
                Debug.Log($"Player defended against {attackData.AttackType} attack!");
                OnPlayerDefendedSuccessfully(attackData);
                continue;
            }

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeHit(attackData.Damage);
                Vector2 dir = ((Vector2)(hit.transform.position - transform.position)).normalized;
                damageable.TakeKnockback(attackData.KnockbackForce, dir);
                Debug.Log($"Boss dealt {attackData.Damage} damage with {attackData.AttackType} attack");
            }
        }
    }

    /// <summary>
    /// Check if the player defended correctly based on attack type.
    /// </summary>
    private bool DidPlayerDefendCorrectly(AttackType attackType)
    {
        if (_playerCombat == null) return false;

        return attackType switch
        {
            AttackType.ParryOnly => _playerCombat.IsParrying(),
            AttackType.DodgeOnly => _playerCombat.IsDashing(),
            AttackType.Both => _playerCombat.IsParrying() || _playerCombat.IsDashing(),
            _ => false
        };
    }

    /// <summary>
    /// Called when player successfully defends. Override or extend for counter-attack opportunities.
    /// </summary>
    protected virtual void OnPlayerDefendedSuccessfully(AttackData attackData)
    {
        // You can trigger stagger, counter-attack window, etc.
        if (_animator != null)
            _animator.SetTrigger("Blocked");
    }

    /// <summary>
    /// Start telegraph flash for an attack. Call this from animation events before the attack hits.
    /// </summary>
    public void StartTelegraph(AttackData attackData)
    {
        if (_telegraph != null && attackData != null)
        {
            _telegraph.StartTelegraph(attackData.AttackType, attackData.TelegraphDuration);
        }
    }

    private IEnumerator DashRoutine(float dashTime, float distance)
    {
        float direction = _bossMovement.IsFacingRight ? 1f : -1f;
        float dashVelocity = distance / dashTime;
        _bossMovement.Rb.linearVelocity = new Vector2(direction * dashVelocity, 0f);

        yield return new WaitForSeconds(dashTime);
        _bossMovement.Rb.linearVelocity = new Vector2(_bossMovement.Rb.linearVelocity.x, _bossMovement.Rb.linearVelocity.y);
    }

    #endregion

    #region Combo Attack

    private Collider2D GetCurrentComboCollider(int index)
    {
        if (index >= 0 && index < _comboAttackColliders.Count)
            return _comboAttackColliders[index];
        return null;
    }

    /// <summary>
    /// Call from animation event to start telegraph for current combo attack.
    /// </summary>
    public void TelegraphComboAttack()
    {
        if (CurrentAttackIndex < 0 || CurrentAttackIndex >= _bossCombatStats.ComboAttacksData.Count) return;
        StartTelegraph(_bossCombatStats.ComboAttacksData[CurrentAttackIndex]);
    }

    public void DashForComboAttack()
    {
        StopAllCoroutines();
        if (CurrentAttackIndex < 0 || CurrentAttackIndex >= _bossCombatStats.ComboAttacksData.Count) return;
        var data = _bossCombatStats.ComboAttacksData[CurrentAttackIndex];
        StartCoroutine(DashRoutine(data.DashDuration, data.DashDistance));
    }

    public void PerformComboAttack()
    {
        if (CurrentAttackIndex < 0 || CurrentAttackIndex >= _bossCombatStats.ComboAttacksData.Count) return;
        AttackData data = _bossCombatStats.ComboAttacksData[CurrentAttackIndex];
        PerformAttack(data, GetCurrentComboCollider(CurrentAttackIndex));
    }

    public void EndComboAttack()
    {
        _comboAttackState.IsAttacking = false;
    }

    #endregion

    #region Heavy Attack

    /// <summary>
    /// Call from animation event to start telegraph for heavy attack.
    /// </summary>
    public void TelegraphHeavyAttack()
    {
        StartTelegraph(_bossCombatStats.HeavyAttackData);
    }

    public void PerformHeavyAttack()
    {
        PerformAttack(_bossCombatStats.HeavyAttackData, _heavyAttackCollider);
    }

    public void DashForHeavyAttack()
    {
        StopAllCoroutines();
        var data = _bossCombatStats.HeavyAttackData;
        StartCoroutine(DashRoutine(data.DashDuration, data.DashDistance));
    }

    #endregion

    #region Dash Attack

    public void ResetDashStateExitTime()
    {
        _dashState.TimeSinceExit = 0f;
    }

    /// <summary>
    /// Call from animation event to start telegraph for dash attack.
    /// </summary>
    public void TelegraphDashAttack()
    {
        StartTelegraph(_bossCombatStats.DashAttackData);
    }

    public void DashForDashAttack()
    {
        StopAllCoroutines();
        var data = _bossCombatStats.DashAttackData;
        StartCoroutine(DashRoutine(data.DashDuration, data.DashDistance));
    }

    public void PerformDashAttack()
    {
        PerformAttack(_bossCombatStats.DashAttackData, _dashAttackCollider);
    }

    #endregion

    public void ExitCombatState()
    {
        ChangeState(_idleState);
    }

    public void EnterDeadState()
    {
        ChangeState(_deadState);
    }
}
