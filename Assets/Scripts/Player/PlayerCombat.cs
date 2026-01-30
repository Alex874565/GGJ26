using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerCombatStats _playerCombatStats;

    private PlayerState _combatState;
    private PlayerHeavyAttackState _heavyAttackState;
    private PlayerComboAttackState _comboAttackState;
    private PlayerParryState _parryState;
    private PlayerDashState _dashState;
    private PlayerIdleState _idleState;
    private PlayerCounterAttackState _counterAttackState;
    private PlayerDashAttackState _dashAttackState;

    private void Start()
    {
        _heavyAttackState = new PlayerHeavyAttackState();
        _comboAttackState = new PlayerComboAttackState();
        _parryState = new PlayerParryState();
        _dashState = new PlayerDashState();
        _idleState = new PlayerIdleState();
        _counterAttackState = new PlayerCounterAttackState();
        _dashAttackState = new PlayerDashAttackState();

        _combatState = _idleState;
    }

    private void Update()
    {
        CheckInput();
        CheckStateTransitions();

        _combatState.Play();
    }


    private void CheckInput()
    {
        if (InputManager.HeavyAttackWasPressed)
        {
            _heavyAttackState.BufferTimer += _playerCombatStats.HeavyAttackBufferTime;
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
        if (_combatState is PlayerIdleState)
        {
            if (_heavyAttackState.BufferTimer > 0)
            {
                ChangeState(_heavyAttackState);
                _heavyAttackState.BufferTimer = 0;
            }
            else if (_comboAttackState.BufferTimer > 0)
            {
                ChangeState(_comboAttackState);
                _comboAttackState.BufferTimer = 0;
            }
            else if (_parryState.BufferTimer > 0)
            {
                ChangeState(_parryState);
                _parryState.BufferTimer = 0;
            }
            else if (_dashState.BufferTimer > 0)
            {
                ChangeState(_dashState);
                _dashState.BufferTimer = 0;
            }
        }
    }

    private void ChangeState(PlayerState state)
    {
        _combatState.Exit();
        _combatState = state;
        _combatState.Enter();
    }

    public void ExitState()
    {
        ChangeState(_idleState);
    }
}