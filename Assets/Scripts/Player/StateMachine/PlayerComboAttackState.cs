// `Assets/Scripts/Player/StateMachine/PlayerComboAttackState.cs`
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboAttackState : PlayerState
{
    public float BufferTimer;

    private float _timeSinceLastAttack;
    private List<AttackData> _comboAttacks => playerCombat.PlayerCombatStats.ComboAttacksData;

    public int CurrentAttackIndex;
    public bool IsAttacking;

    public PlayerComboAttackState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
    }

    public override void Enter()
    {
        _timeSinceLastAttack = 0f;
        playerMovement.Rb.linearVelocity = Vector2.zero;

        CurrentAttackIndex = 0;
        IsAttacking = true;

        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Combo Attack");
        // If your animator needs a trigger to enter Attack 1, add it here; otherwise rely on transitions.
        // playerCombat.Animator.SetTrigger("Attack 1");
    }

    public override void Update()
    {
        if (IsAttacking)
            return;

        _timeSinceLastAttack += Time.deltaTime;

        // buffered next attack within the allowed window
        if (_timeSinceLastAttack >= playerCombat.PlayerCombatStats.BetweenAttackCooldown
            && BufferTimer > 0f)
        {
            BufferTimer = 0f;
            _timeSinceLastAttack = 0f;

            // Advance combo index BEFORE triggering so transitions can use it (if you use an int parameter)
            CurrentAttackIndex++;

            if (CurrentAttackIndex < _comboAttacks.Count)
            {
                IsAttacking = true;
                playerCombat.Animator.SetTrigger("Next Attack");
            }
            else
            {
                playerCombat.ExitState();
            }

            return;
        }

        // combo window expired
        if (_timeSinceLastAttack >= playerCombat.PlayerCombatStats.MaxTimeBetweenAttacks)
        {
            playerCombat.ExitState();
        }
    }

    public override void FixedUpdate()
    {
        if (IsAttacking)
        {
            // Lock horizontal velocity unless dash is actively driving it
            playerMovement.Rb.linearVelocity =
                new Vector2(playerMovement.Rb.linearVelocity.x, 0f);
        }
    }


    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsAttacking", false);
        IsAttacking = false;
        CurrentAttackIndex = -1;
    }
}
