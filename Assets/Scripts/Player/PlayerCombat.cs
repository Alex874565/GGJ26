using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator Animator => _animator;
    public PlayerState PlayerCombatState => _combatState;
    public PlayerCombatStats PlayerCombatStats => _playerCombatStats;
    public PlayerManager PlayerManager => _playerManager;

    [SerializeField] private PlayerCombatStats _playerCombatStats;
    [Header("Colliders")]
    [SerializeField] private List<Collider2D> _comboAttackColliders;
    [SerializeField] private Collider2D _dashAttackCollider;
    [SerializeField] private Collider2D _heavyAttackCollider;
    [SerializeField] private Collider2D _counterAttackCollider;
    [SerializeField] private Collider2D _jumpAttackCollider;

    private Animator _animator;
    private PlayerMovement _playerMovement;
    
    private int CurrentAttackIndex => (_combatState is PlayerComboAttackState comboState) ? comboState.CurrentAttackIndex : -1;

    private PlayerState _combatState;
    private PlayerHeavyAttackState _heavyAttackState;
    private PlayerComboAttackState _comboAttackState;
    private PlayerParryState _parryState;
    private PlayerDashState _dashState;
    private PlayerIdleState _idleState;
    private PlayerCounterAttackState _counterAttackState;
    private PlayerDashAttackState _dashAttackState;
    private PlayerAirAttackState _airAttackState;
    private PlayerDownedState _downedState;
    private PlayerHealState _healState;

    private int _potionCount;

    private PlayerManager _playerManager => ServiceLocator.Instance.PlayerManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    
    private void Start()
    {
        _heavyAttackState = new PlayerHeavyAttackState(_playerMovement, this);
        _comboAttackState = new PlayerComboAttackState(_playerMovement, this);
        _parryState = new PlayerParryState(_playerMovement, this);
        _dashState = new PlayerDashState(_playerMovement, this);
        _idleState = new PlayerIdleState(_playerMovement, this);
        _counterAttackState = new PlayerCounterAttackState(_playerMovement, this);
        _dashAttackState = new PlayerDashAttackState(_playerMovement, this);
        _airAttackState = new PlayerAirAttackState(_playerMovement, this);
        _downedState = new PlayerDownedState(_playerMovement, this, _playerCombatStats.DownedDuration, _playerCombatStats.DownedGracePeriod);
        _healState = new PlayerHealState(_playerMovement, this, _playerCombatStats.HealDuration);

        _potionCount = _playerCombatStats.StartingPotions;
        _combatState = _idleState;
    }
    
    private void Update()
    {
        ChangeTimers();
        CheckInput();
        CheckStateTransitions();

        _combatState.Update();
    }

    private void FixedUpdate()
    {
        _combatState.FixedUpdate();
    }
    
    private void CheckInput()
    {
        _heavyAttackState.IsBeingHeld = InputManager.HeavyAttackIsHeld;
        _parryState.IsBeingHeld = InputManager.ParryIsHeld;
        if (InputManager.AttackWasPressed)
        {
            // During dash - always dash attack
            if(_combatState is PlayerDashState)
            {
                _dashAttackState.BufferTimer = _playerCombatStats.BufferTime;
                return;
            }
            // Shortly after dash ended - dash attack takes priority
            else if (_dashState.TimeSinceExit < _playerCombatStats.AfterDashAttackDelay)
            {
                _dashAttackState.BufferTimer = _playerCombatStats.BufferTime;
                return;
            }
            else if (_combatState is PlayerParryState)
            {
                _counterAttackState.BufferTimer = _playerCombatStats.BufferTime;
                return;
            }
            else
            {
                if (_playerMovement.IsGrounded)
                {
                    _comboAttackState.BufferTimer = _playerCombatStats.BufferTime;
                }
                else
                {
                    _airAttackState.BufferTimer = _playerCombatStats.BufferTime;
                }
            }
        }
        else if (InputManager.DashWasPressed)
        {
            _dashState.BufferTimer = _playerCombatStats.BufferTime;
        }
    }

    private void CheckStateTransitions()
    {
        if(ShouldEnterDash() && _combatState is not PlayerDashState)
        {
            Debug.Log("Entering Dash State");
            ChangeState(_dashState);
            _dashState.BufferTimer = 0;
            return;
        }

        switch (_combatState)
        {
            case PlayerIdleState:
                if (ShouldEnterHeal())
                {
                    ChangeState(_healState);
                }
                else if (ShouldEnterHeavyAttack())
                {
                    ChangeState(_heavyAttackState);
                }
                else if (ShouldEnterDashAttack())
                {
                    // Check dash attack before combo - allows attack right after dash ends
                    ChangeState(_dashAttackState);
                    _dashAttackState.BufferTimer = 0;
                }
                else if (ShouldEnterComboAttack())
                {
                    ChangeState(_comboAttackState);
                    _comboAttackState.BufferTimer = 0;
                }
                else if (ShouldParry())
                {
                    ChangeState(_parryState);
                }
                else if (ShouldEnterAirAttack())
                {
                    ChangeState(_airAttackState);
                    _airAttackState.BufferTimer = 0;
                }
                break;

            case PlayerDashState:
                if (ShouldEnterDashAttack())
                {
                    ChangeState(_dashAttackState);
                    _dashAttackState.BufferTimer = 0;
                }
                break;
            
            case PlayerParryState:
                if (ShouldEnterCounterAttack())
                {
                    ChangeState(_counterAttackState);
                    _counterAttackState.BufferTimer = 0;
                }
                else if (!_parryState.IsBeingHeld && _parryState.ParryRaised)
                {
                    _parryState.LowerParry();
                }

                break;
        }
    }

    private void ChangeTimers()
    {
        _comboAttackState.BufferTimer -= Time.deltaTime;
        _dashState.BufferTimer -= Time.deltaTime;
        _dashAttackState.BufferTimer -= Time.deltaTime;

        _dashState.TimeSinceExit += Time.deltaTime;
        _comboAttackState.TimeSinceExit += Time.deltaTime;
        _heavyAttackState.TimeSinceExit += Time.deltaTime;
        _airAttackState.TimeSinceExit += Time.deltaTime;
        _downedState.UpdateGraceTimer(Time.deltaTime);
    }

    private void ChangeState(PlayerState state)
    {
        Debug.Log($"Changing state from {_combatState.GetType().Name} to {state.GetType().Name}");
        _combatState.Exit();
        _combatState = state;
        _combatState.Enter();
    }

    #region Should Enter State Checks
    
    public bool ShouldEnterAirAttack()
    {
        return _airAttackState.BufferTimer > 0 && !_playerMovement.IsGrounded && _airAttackState.TimeSinceExit > _playerCombatStats.AirAttackCooldown;
    }
    
    public bool ShouldEnterHeavyAttack()
    {
        return _heavyAttackState.IsBeingHeld && !_heavyAttackState.IsAttacking && _heavyAttackState.TimeSinceExit > _playerCombatStats.HeavyAttackCooldown && _playerMovement.IsGrounded;
    }
    public bool ShouldEnterComboAttack()
    {
        return _comboAttackState.BufferTimer > 0 && _comboAttackState.TimeSinceExit > _playerCombatStats.ComboAttackCooldown;
    }
    public bool ShouldParry()
    {
        return _parryState.IsBeingHeld && !_parryState.ParryRaised && _playerMovement.IsGrounded;
    }
    public bool ShouldEnterDash()
    {
        return _dashState.BufferTimer > 0 && _dashState.TimeSinceExit > _dashState.Cooldown;
    }
    public bool ShouldEnterDashAttack()
    {
        return _dashAttackState.BufferTimer > 0 && (_combatState is PlayerDashState || _dashState.TimeSinceExit < _playerCombatStats.AfterDashAttackDelay);
    }
    public bool ShouldEnterHeal()
    {
        return InputManager.HealWasPressed && _potionCount > 0 && _playerMovement.IsGrounded && _playerManager.CurrentHealth < _playerCombatStats.Health && !IsHealing();
    }
    public bool ShouldEnterCounterAttack()
    {
        return _counterAttackState.BufferTimer > 0 && _combatState is PlayerParryState && _counterAttackState.LastSuccessfulParryTime < _playerCombatStats.CounterAttackWindow;
    }
    #endregion

    #region General Combat Methods

    private void PerformAttack(AttackData attackData, Collider2D attackCollider)
    {
        if (attackCollider == null) return;

        // Create a contact filter to only detect enemies
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.SetLayerMask(LayerMask.GetMask("Enemy")); // Only detect Enemy layer

        // Detect all enemies overlapping with the attack collider
        List<Collider2D> hitEnemies = new List<Collider2D>();
        int hitCount = Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);

        Debug.Log($"Attack hit {hitCount} enemies");

        // Apply damage to each hit enemy
        foreach (Collider2D enemy in hitEnemies)
        {
            // Check if enemy (boss) is parrying
            BossCombat bossCombat = enemy.GetComponent<BossCombat>();
            if (bossCombat != null && bossCombat.IsParrying())
            {
                Debug.Log($"Attack was parried by {enemy.name}!");
                OnAttackParried(enemy);
                continue;
            }

            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeHit(attackData.Damage);
                Vector2 dir = ((Vector2)(enemy.transform.position - transform.position)).normalized;
                damageable.TakeKnockback(attackData.KnockbackForce, dir, attackData.StunChance);
                Debug.Log($"Dealt {attackData.Damage} damage to {enemy.name}");
            }
        }
    }

    /// <summary>
    /// Called when player's attack is parried by an enemy.
    /// </summary>
    private void OnAttackParried(Collider2D enemy)
    {
        // Trigger stagger or recoil animation
        if (_animator != null)
            _animator.SetTrigger("Blocked");
    }


    private IEnumerator DashRoutine(float dashTime, float distance)
    {
        float direction = _playerMovement.IsFacingRight ? 1f : -1f;
        float dashVelocity = distance / dashTime;
        _playerMovement.Rb.linearVelocity = new Vector2(direction * dashVelocity, 0f);

        yield return new WaitForSeconds(dashTime);
            
        // Reduce momentum after dash ends - keeps some velocity for fluid feel
        _playerMovement.Rb.linearVelocity = new Vector2(
            _playerMovement.Rb.linearVelocity.x * 0.05f, 
            _playerMovement.Rb.linearVelocity.y
        );
    }

    public void SuccessfulParry()
    {
        _counterAttackState.LastSuccessfulParryTime = 0;
    }

    #endregion

    #region Combo Attacks

    private Collider2D GetCurrentAttackCollider(int attackIndex)
    {
        if (attackIndex >= 0 && attackIndex < _comboAttackColliders.Count)
        {
            return _comboAttackColliders[attackIndex];
        }
        return null;
    }

    public void DashForComboAttack()
    {
        StopAllCoroutines();
        
        float dashTime = _playerCombatStats.ComboAttacksData[CurrentAttackIndex].DashDuration;
        float distance = _playerCombatStats.ComboAttacksData[CurrentAttackIndex].DashDistance;

        StartCoroutine(DashRoutine(dashTime, distance));
    }
    
    public void PerformComboAttack()
    {
        AttackData attackData = _playerCombatStats.ComboAttacksData[CurrentAttackIndex];
        Collider2D attackCollider = GetCurrentAttackCollider(CurrentAttackIndex);
        PerformAttack(attackData, attackCollider);
    }
    
    public void EndAttack()
    {
        _comboAttackState.IsAttacking = false;
    }
    
    #endregion
    
    #region Dash Attack

    public void ResetDashStateExitTime()
    {
        _dashState.TimeSinceExit = 0;
    }
    
    public void SetDashExitTimePastWindow()
    {
        // Set past the AfterDashAttackDelay so we can't chain dash attacks
        _dashState.TimeSinceExit = _playerCombatStats.AfterDashAttackDelay + 0.1f;
    }
    
    public void DashForDashAttack()
    {
        StopAllCoroutines();
        
        float dashTime = _playerCombatStats.DashAttackData.DashDuration;
        float distance = _playerCombatStats.DashAttackData.DashDistance;

        StartCoroutine(DashRoutine(dashTime, distance));
    }
    
    public void PerformDashAttack()
    {
        AttackData attackData = _playerCombatStats.DashAttackData;
        PerformAttack(attackData, _dashAttackCollider);
    }
    
    #endregion

    #region Air Attack
    
        private IEnumerator DiagonalDownDashRoutine(float dashTime, float distance)
        {
            Rigidbody2D rb = _playerMovement.Rb;

            float forward = _playerMovement.IsFacingRight ? 1f : -1f;
            Vector2 dashDirection = new Vector2(forward, -1f).normalized;
            float dashSpeed = distance / dashTime;

            rb.linearVelocity = dashDirection * dashSpeed;

            yield return new WaitForSeconds(dashTime);
            
            // Reduce momentum after dash ends - keeps some velocity for fluid feel
            rb.linearVelocity = rb.linearVelocity * 0.15f;
        }
        
        public void PerformAirAttack()
        {
            AttackData attackData = _playerCombatStats.AirAttackData;
            PerformAttack(attackData, _jumpAttackCollider);
        }
        
        public void DashForAirAttack()
        {
            StopAllCoroutines();
        
            float dashTime = _playerCombatStats.AirAttackData.DashDuration;
            float distance = _playerCombatStats.AirAttackData.DashDistance;

            StartCoroutine(DiagonalDownDashRoutine(dashTime, distance));
        }
        
        public void RemoveAirAttackCooldown()
        {
            _airAttackState.TimeSinceExit = _playerCombatStats.AirAttackCooldown;
        }

    #endregion

    #region Heavy Attack

        public void PerformHeavyAttack()
        {
            AttackData attackData = _playerCombatStats.HeavyAttackData;
            PerformAttack(attackData, _heavyAttackCollider);
        }
        
        public void DashForHeavyAttack()
        {
            StopAllCoroutines();
        
            float dashTime = _playerCombatStats.HeavyAttackData.DashDuration;
            float distance = _playerCombatStats.HeavyAttackData.DashDistance;

            StartCoroutine(DashRoutine(dashTime, distance));
        }

    #endregion

    #region Counter Attack
    public void PerformCounterAttack()
    {
        AttackData attackData = _playerCombatStats.CounterAttackData;
        PerformAttack(attackData, _counterAttackCollider);
    }
    #endregion

    public void ExitState()
    {
        StopAllCoroutines(); // Stop any dash routines that might interfere with movement
        ChangeState(_idleState);
    }

    #region Defense State Checks (for boss attack resolution)

    /// <summary>
    /// Returns true if player is currently in parry state with parry raised.
    /// </summary>
    public bool IsParrying()
    {
        return _combatState is PlayerParryState parryState && parryState.ParryRaised;
    }

    /// <summary>
    /// Returns true if player is currently dashing (invulnerable to dodge-only attacks).
    /// </summary>
    public bool IsDashing()
    {
        return _combatState is PlayerDashState;
    }

    /// <summary>
    /// Returns true if player is currently in downed state.
    /// </summary>
    public bool IsDowned()
    {
        return _combatState is PlayerDownedState;
    }

    #endregion

    #region Downed State

    /// <summary>
    /// Forces the player into the downed state. Stops all movement and input for the downed duration.
    /// </summary>
    public void EnterDownedState()
    {
        if (_combatState is PlayerDownedState) return; // Already downed
        if (_downedState.IsInGracePeriod) return; // Still in grace period
        
        StopAllCoroutines();
        ChangeState(_downedState);
    }

    #endregion

    #region Healing

    public int PotionCount => _potionCount;

    /// <summary>
    /// Called by PlayerHealState to apply the heal effect and consume a potion.
    /// </summary>
    public void ApplyHeal()
    {
        if (_potionCount <= 0) return;
        
        _potionCount--;
        _playerManager.Heal(_playerCombatStats.HealAmount);
        Debug.Log($"Healed for {_playerCombatStats.HealAmount}. Potions remaining: {_potionCount}");
    }

    /// <summary>
    /// Adds potions to the player's inventory.
    /// </summary>
    public void AddPotions(int amount)
    {
        _potionCount += amount;
    }

    /// <summary>
    /// Returns true if player is currently healing.
    /// </summary>
    public bool IsHealing()
    {
        return _combatState is PlayerHealState;
    }

    #endregion
}