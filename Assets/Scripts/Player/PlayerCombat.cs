using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator Animator => _animator;
    public PlayerState PlayerCombatState => _combatState;
    public PlayerCombatStats PlayerCombatStats => _playerCombatStats;

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
        if (InputManager.HeavyAttackWasPressed)
        {
            _heavyAttackState.BufferTimer = _playerCombatStats.HeavyAttackBufferTime;
        }
        else if (InputManager.AttackWasPressed)
        {
            if(_combatState is PlayerDashState)
            {
                _dashAttackState.BufferTimer = _playerCombatStats.ComboAttackBufferTime;
                return;
            }
            _comboAttackState.BufferTimer = _playerCombatStats.ComboAttackBufferTime;
        }
        else if (InputManager.ParryWasPressed)
        {
            _parryState.BufferTimer = _playerCombatStats.ParryBufferTime;
        }
        else if (InputManager.DashWasPressed)
        {
            _dashState.BufferTimer = _playerCombatStats.DashBufferTime;
        }
    }

    private void CheckStateTransitions()
    {
        if(ShouldEnterDash() && _combatState is not PlayerDashState)
        {
            ChangeState(_dashState);
            _dashState.BufferTimer = 0;
            return;
        }

        switch (_combatState)
        {
            case PlayerIdleState:
                if (ShouldEnterHeavyAttack())
                {
                    ChangeState(_heavyAttackState);
                    _heavyAttackState.BufferTimer = 0;
                }
                else if (ShouldEnterComboAttack())
                {
                    ChangeState(_comboAttackState);
                    _comboAttackState.BufferTimer = 0;
                }
                else if (ShouldEnterParry())
                {
                    ChangeState(_parryState);
                    _parryState.BufferTimer = 0;
                }
                break;

            case PlayerDashState:
                if (ShouldEnterDashAttack())
                {
                    ChangeState(_dashAttackState);
                    _dashAttackState.BufferTimer = 0;
                }
                break;

        }
    }

    private void ChangeTimers()
    {
        _heavyAttackState.BufferTimer -= Time.deltaTime;
        _comboAttackState.BufferTimer -= Time.deltaTime;
        _parryState.BufferTimer -= Time.deltaTime;
        _dashState.BufferTimer -= Time.deltaTime;

        _dashState.TimeSinceExit += Time.deltaTime;
    }

    private void ChangeState(PlayerState state)
    {
        Debug.Log($"Changing state from {_combatState.GetType().Name} to {state.GetType().Name}");
        _combatState.Exit();
        _combatState = state;
        _combatState.Enter();
    }

    #region Should Enter State Checks
    public bool ShouldEnterHeavyAttack()
    {
        return _heavyAttackState.BufferTimer > 0;
    }
    public bool ShouldEnterComboAttack()
    {
        return _comboAttackState.BufferTimer > 0;
    }
    public bool ShouldEnterParry()
    {
        return _parryState.BufferTimer > 0;
    }
    public bool ShouldEnterDash()
    {
        return _dashState.BufferTimer > 0 && _dashState.TimeSinceExit > _dashState.Cooldown;
    }
    public bool ShouldEnterDashAttack()
    {
        return _dashAttackState.BufferTimer > 0;
    }
    #endregion
    
    #region General Combat Methods
    
        private void PerformAttack(AttackData attackData, Collider2D attackCollider)
        {
            if (CurrentAttackIndex >= 0 && CurrentAttackIndex < _playerCombatStats.ComboAttacksData.Count)
            {
                if (attackCollider == null) return;

                // Create a contact filter to only detect enemies
                ContactFilter2D filter = new ContactFilter2D();
                filter.useLayerMask = true;
                filter.SetLayerMask(LayerMask.GetMask("Enemy")); // Only detect Enemy layer

                // Detect all enemies overlapping with the attack collider
                List<Collider2D> hitEnemies = new List<Collider2D>();
                int hitCount = Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);

                Debug.Log($"Attack {CurrentAttackIndex} hit {hitCount} enemies");

                // Apply damage to each hit enemy
                foreach (Collider2D enemy in hitEnemies)
                {
                    IDamageable damageable = enemy.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(attackData.Damage);
                        Debug.Log($"Dealt {attackData.Damage} damage to {enemy.name}");
                    }
                }
            }
        }


        private IEnumerator DashRoutine(float dashTime, float distance)
        {
            float direction = _playerMovement.IsFacingRight ? 1f : -1f;
            float dashVelocity = distance / dashTime;
            _playerMovement.Rb.linearVelocity = new Vector2(direction * dashVelocity, 0f);

            yield return new WaitForSeconds(dashTime);

            // Stop dash
            _playerMovement.Rb.linearVelocity = Vector2.zero;
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
    
    public void ExitState()
    {
        ChangeState(_idleState);
    }
}