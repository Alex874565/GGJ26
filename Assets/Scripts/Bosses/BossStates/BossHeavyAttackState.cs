using UnityEngine;

public class BossHeavyAttackState : BossCombatState
{
    public bool IsAttacking;
    public float TimeSinceExit;
    private float _chargeTimer;
    private float _chargeTime;
    private float _attackPhaseTimer;
    private const float AttackPhaseTimeout = 0.5f;

    public BossHeavyAttackState(BossMovement movement, BossCombat combat) : base(movement, combat)
    {
        TimeSinceExit = 0f;
        _chargeTime = combat.BossCombatStats.HeavyAttackChargeTime;
    }

    public override void Enter()
    {
        bossMovement.SetHorizontalVelocity(0f);
        _chargeTimer = 0f;
        _attackPhaseTimer = 0f;
        IsAttacking = false;
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetTrigger("Charge Heavy Attack");
    }

    public override void Update()
    {
        if (IsAttacking)
        {
            _attackPhaseTimer += Time.deltaTime;
            if (_attackPhaseTimer >= AttackPhaseTimeout)
                bossCombat.ExitCombatState(1);
            return;
        }

        _chargeTimer += Time.deltaTime;
        if (_chargeTimer >= _chargeTime)
        {
            IsAttacking = true;
            _attackPhaseTimer = 0f;
            if (bossCombat.Animator != null)
                bossCombat.Animator.SetBool("IsAttacking", true);
        }
    }

    public override void Exit()
    {
        if (IsAttacking) TimeSinceExit = 0f;
        IsAttacking = false;
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsAttacking", false);
    }
}
