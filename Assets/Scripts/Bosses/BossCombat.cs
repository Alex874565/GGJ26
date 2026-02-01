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

    [Header("Health Reference (for phases)")]
    [SerializeField] private Health _health;

    private Animator _animator;
    private BossMovement _bossMovement;
    private PlayerCombat _playerCombat;

    private int CurrentAttackIndex => (_combatState is BossComboAttackState comboState) ? comboState.CurrentAttackIndex : -1;

    // Phase tracking
    private bool _isPhase2;
    private float _currentIdleTarget;

    private BossCombatState _combatState;
    private BossCombatIdleState _idleState;
    private BossComboAttackState _comboAttackState;
    private BossHeavyAttackState _heavyAttackState;
    private BossDashState _dashState;
    private BossDashAttackState _dashAttackState;
    private BossParryState _parryState;
    private BossDeadState _deadState;
    private BossStunState _stunState;

    public bool IsPhase2 => _isPhase2;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bossMovement = GetComponent<BossMovement>();
        
        if (_telegraph == null)
            _telegraph = GetComponent<BossTelegraph>();
        
        if (_health == null)
            _health = GetComponent<Health>();
    }

    private void Start()
    {
        _idleState = new BossCombatIdleState(_bossMovement, this);
        _comboAttackState = new BossComboAttackState(_bossMovement, this);
        _heavyAttackState = new BossHeavyAttackState(_bossMovement, this);
        _dashState = new BossDashState(_bossMovement, this);
        _dashAttackState = new BossDashAttackState(_bossMovement, this);
        _parryState = new BossParryState(_bossMovement, this);
        _deadState = new BossDeadState(_bossMovement, this);
        _stunState = new BossStunState(_bossMovement, this, _bossCombatStats.StunDuration, _bossCombatStats.StunGracePeriod);

        _combatState = _idleState;
        _combatState.Enter();

        // Initialize random idle target
        _currentIdleTarget = Random.Range(
            _bossCombatStats.MinIdleTimeBeforeAttack,
            _bossCombatStats.MaxIdleTimeBeforeAttack
        );
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
        // Check phase transition
        if (!_isPhase2 && _health != null)
        {
            float healthPercent = _health.currentHealth / _health.maxHealth;
            if (healthPercent <= _bossCombatStats.Phase2HealthThreshold)
            {
                EnterPhase2();
            }
        }
    }

    private void EnterPhase2()
    {
        _isPhase2 = true;
        if (_animator != null)
            _animator.SetTrigger("Phase2");
        Debug.Log("Boss entered Phase 2!");
    }

    /// <summary>
    /// Get the aggression multiplier based on current phase.
    /// </summary>
    private float GetAggressionMultiplier()
    {
        return _isPhase2 ? _bossCombatStats.Phase2AggressionMultiplier : 1f;
    }

    private void CheckStateTransitions()
    {
        if (_combatState is BossDeadState) return;

        float dist = _bossMovement.DistanceToTarget();
        var idleState = _idleState;

        switch (_combatState)
        {
            case BossCombatIdleState:
                if (!_bossMovement.IsGrounded)
                {
                    Debug.Log($"[Boss] Not grounded, can't attack");
                    break;
                }

                // Priority: Parry if player is attacking nearby
                if (dist <= _bossCombatStats.ParryRange && ShouldEnterParry())
                {
                    ChangeState(_parryState);
                    break;
                }

                // Random idle time (affected by aggression)
                float minIdle = _bossCombatStats.MinIdleTimeBeforeAttack / GetAggressionMultiplier();
                if (idleState.TimeInIdle < minIdle) break;
                
                // Check if we've waited long enough for this idle period
                if (idleState.TimeInIdle < _currentIdleTarget) break;

                // Choose attack based on weighted random selection
                BossCombatState chosenAttack = ChooseAttack(dist);
                if (chosenAttack != null)
                {
                    Debug.Log($"[Boss] Choosing attack: {chosenAttack.GetType().Name} at distance {dist}");
                    ChangeState(chosenAttack);
                    // Set new random idle target for next time
                    _currentIdleTarget = Random.Range(
                        _bossCombatStats.MinIdleTimeBeforeAttack,
                        _bossCombatStats.MaxIdleTimeBeforeAttack
                    ) / GetAggressionMultiplier();
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

    /// <summary>
    /// Choose an attack based on weighted random selection and distance.
    /// </summary>
    private BossCombatState ChooseAttack(float distanceToPlayer)
    {
        // Build list of valid attacks with their weights
        List<(BossCombatState state, float weight)> validAttacks = new List<(BossCombatState, float)>();

        // Combo - close range
        bool comboInRange = distanceToPlayer <= _bossCombatStats.ComboAttackRange;
        bool comboCooldownReady = ShouldEnterComboAttack();
        if (comboInRange && comboCooldownReady)
        {
            validAttacks.Add((_comboAttackState, _bossCombatStats.ComboWeight));
        }

        // Heavy - medium range
        bool heavyInRange = distanceToPlayer <= _bossCombatStats.HeavyAttackRange;
        bool heavyCooldownReady = ShouldEnterHeavyAttack();
        if (heavyInRange && heavyCooldownReady)
        {
            validAttacks.Add((_heavyAttackState, _bossCombatStats.HeavyWeight));
        }

        // Dash (to close gap) - long range
        bool dashInRange = distanceToPlayer <= _bossCombatStats.DashRange && distanceToPlayer > _bossCombatStats.ComboAttackRange;
        bool dashCooldownReady = ShouldEnterDash();
        if (dashInRange && dashCooldownReady)
        {
            validAttacks.Add((_dashState, _bossCombatStats.DashWeight));
        }

        // Dash attack - if just dashed
        if (ShouldEnterDashAttack())
        {
            validAttacks.Add((_dashAttackState, _bossCombatStats.DashWeight));
        }

        if (validAttacks.Count == 0) return null;

        // Weighted random selection
        float totalWeight = 0f;
        foreach (var attack in validAttacks)
            totalWeight += attack.weight;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var attack in validAttacks)
        {
            cumulative += attack.weight;
            if (randomValue <= cumulative)
                return attack.state;
        }

        return validAttacks[0].state;
    }

    private void ChangeTimers()
    {
        _comboAttackState.TimeSinceExit += Time.deltaTime;
        _heavyAttackState.TimeSinceExit += Time.deltaTime;
        _dashState.TimeSinceExit += Time.deltaTime;
        _parryState.TimeSinceExit += Time.deltaTime;
        _stunState.UpdateGraceTimer(Time.deltaTime);
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

    public bool ShouldEnterParry()
    {
        if (_playerCombat == null) return false;
        
        // Check if player is currently attacking
        bool playerIsAttacking = _playerCombat.PlayerCombatState is PlayerComboAttackState
            || _playerCombat.PlayerCombatState is PlayerHeavyAttackState
            || _playerCombat.PlayerCombatState is PlayerDashAttackState
            || _playerCombat.PlayerCombatState is PlayerAirAttackState;
        
        if (!playerIsAttacking) return false;
        if (!_bossMovement.IsGrounded) return false;
        if (_parryState.TimeSinceExit <= _bossCombatStats.ParryCooldown) return false;
        
        // Random chance to parry (higher in phase 2)
        float parryChance = _bossCombatStats.ParryChance * GetAggressionMultiplier();
        return Random.value <= parryChance;
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
            Debug.Log($"Boss attack hit: {hit.name}");
            // Check if player defended correctly
            if (DidPlayerDefendCorrectly(attackData.AttackType))
            {
                Debug.Log($"Player defended against {attackData.AttackType} attack!");
                if (_playerCombat.IsParrying())
                {
                    OnPlayerParriedSuccessfully(attackData);
                }
                continue;
            }

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeHit(attackData.Damage);
                Vector2 dir = ((Vector2)(hit.transform.position - transform.position)).normalized;
                damageable.TakeKnockback(attackData.KnockbackForce, dir, attackData.StunChance);
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

        if (_playerCombat.IsParrying())
        {
            _playerCombat.SuccessfulParry();
        }

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
    protected virtual void OnPlayerParriedSuccessfully(AttackData attackData)
    {
        // You can trigger stagger, counter-attack window, etc.
        if (_animator != null)
            _animator.SetTrigger("Blocked");

        _playerCombat.SuccessfulParry();
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
        
        // Take control of velocity from BossMovement
        _bossMovement.ExternalVelocityControl = true;
        
        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            _bossMovement.Rb.linearVelocity = new Vector2(direction * dashVelocity, 0f);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        _bossMovement.Rb.linearVelocity = new Vector2(0f, _bossMovement.Rb.linearVelocity.y);
        
        // Return control to BossMovement
        _bossMovement.ExternalVelocityControl = false;
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
    
    public void ExitToIdle()
    {
        StopAllCoroutines();
        ChangeState(_idleState);
    }

    public void EnterDeadState()
    {
        ChangeState(_deadState);
    }

    #region Defense State Checks (for player attack resolution)

    /// <summary>
    /// Returns true if boss is currently parrying.
    /// </summary>
    public bool IsParrying()
    {
        return _combatState is BossParryState parryState && parryState.ParryRaised;
    }

    /// <summary>
    /// Returns true if boss is currently stunned.
    /// </summary>
    public bool IsStunned()
    {
        return _combatState is BossStunState;
    }

    #endregion

    #region Stun State

    /// <summary>
    /// Forces the boss into the stun state. Stops all attacks and movement for the stun duration.
    /// </summary>
    public void EnterStunState()
    {
        if (_combatState is BossStunState) return; // Already stunned
        if (_combatState is BossDeadState) return; // Can't stun if dead
        if (_stunState.IsInGracePeriod) return; // Still in grace period
        
        StopAllCoroutines();
        ChangeState(_stunState);
    }

    #endregion
}
