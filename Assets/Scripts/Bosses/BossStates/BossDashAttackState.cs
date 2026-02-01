using UnityEngine;

public class BossDashAttackState : BossCombatState
{
    public BossDashAttackState(BossMovement movement, BossCombat combat) : base(movement, combat) { }

    public override void Enter()
    {
        if (bossCombat.Animator != null)
        {
            bossCombat.Animator.SetBool("IsAttacking", true);
            bossCombat.Animator.SetTrigger("Dash Attack");
        }
        // Lunge immediately so dash attack follows right after dash (don't wait for animation event)
        bossCombat.DashForDashAttack();
    }

    public override void Exit()
    {
        bossCombat.ResetDashStateExitTime();
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsAttacking", false);
    }
}
