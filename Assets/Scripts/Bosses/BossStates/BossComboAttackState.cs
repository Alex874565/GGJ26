using System.Collections.Generic;
using UnityEngine;

public class BossComboAttackState : BossCombatState
{
    public int CurrentAttackIndex;
    public bool IsAttacking;
    public float TimeSinceExit;
    private float _timeSinceLastAttack;
    private List<AttackData> _comboAttacks => bossCombat.BossCombatStats?.ComboAttacksData;

    public BossComboAttackState(BossMovement movement, BossCombat combat) : base(movement, combat)
    {
        TimeSinceExit = 0f;
    }

    public override void Enter()
    {
        _timeSinceLastAttack = 0f;
        bossMovement.SetHorizontalVelocity(0f);
        CurrentAttackIndex = 0;
        IsAttacking = true;
        if (bossCombat.Animator != null)
        {
            bossCombat.Animator.SetBool("IsAttacking", true);
            bossCombat.Animator.SetTrigger("Combo Attack");
        }
    }

    public override void Update()
    {
        if (IsAttacking) return;

        _timeSinceLastAttack += Time.deltaTime;

        if (_timeSinceLastAttack >= bossCombat.BossCombatStats.MaxTimeBetweenAttacks)
        {
            // Only recharge if did at least 2 attacks (CurrentAttackIndex 1 = 1 attack, 2+ = 2+ attacks)
            int triggerRecharge = CurrentAttackIndex >= 2 ? 1 : 0;
            bossCombat.ExitCombatState(triggerRecharge);
            return;
        }

        if (_timeSinceLastAttack >= bossCombat.BossCombatStats.BetweenAttackCooldown)
        {
            // Check if player is still in range before continuing combo
            float distanceToPlayer = bossMovement.DistanceToTarget();
            if (distanceToPlayer > bossCombat.BossCombatStats.ComboAttackRange)
            {
                // Only recharge if did at least 2 attacks (CurrentAttackIndex 0 = 1 attack done)
                int triggerRecharge = CurrentAttackIndex >= 1 ? 1 : 0;
                bossCombat.ExitCombatState(triggerRecharge);
                return;
            }

            _timeSinceLastAttack = 0f;
            CurrentAttackIndex++;

            if (_comboAttacks != null && CurrentAttackIndex < _comboAttacks.Count && bossCombat.Animator != null)
            {
                IsAttacking = true;
                bossCombat.Animator.SetTrigger("Next Attack");
            }
            else
            {
                // Combo finished - recharge only if did at least 2 attacks (CurrentAttackIndex 2+ = 2+ attacks)
                int triggerRecharge = CurrentAttackIndex >= 2 ? 1 : 0;
                bossCombat.ExitCombatState(triggerRecharge);
            }
        }
    }

    public override void FixedUpdate()
    {
        // Keep horizontal velocity at 0 during attack
        if (IsAttacking)
        {
            bossMovement.SetHorizontalVelocity(0f);
        }
    }

    public override void Exit()
    {
        TimeSinceExit = 0f;
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsAttacking", false);
        IsAttacking = false;
        CurrentAttackIndex = -1;
    }
}
