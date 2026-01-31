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
    }

    public override void Exit()
    {
        bossCombat.ResetDashStateExitTime();
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsAttacking", false);
    }
}
