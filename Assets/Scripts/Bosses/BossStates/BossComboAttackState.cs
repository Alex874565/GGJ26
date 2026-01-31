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
        bossMovement.Rb.linearVelocity = Vector2.zero;
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
            bossCombat.ExitCombatState();
            return;
        }

        if (_timeSinceLastAttack >= bossCombat.BossCombatStats.BetweenAttackCooldown)
        {
            _timeSinceLastAttack = 0f;
            CurrentAttackIndex++;

            if (_comboAttacks != null && CurrentAttackIndex < _comboAttacks.Count && bossCombat.Animator != null)
            {
                IsAttacking = true;
                bossCombat.Animator.SetTrigger("Next Attack");
            }
            else
            {
                bossCombat.ExitCombatState();
            }
        }
    }

    public override void FixedUpdate()
    {
        if (IsAttacking)
        {
            bossMovement.Rb.linearVelocity = new Vector2(bossMovement.Rb.linearVelocity.x, 0f);
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
