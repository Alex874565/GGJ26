using UnityEngine;

public class PlayerDashAttackState : PlayerState
{
    public float BufferTimer;
    private float _timer;

    public PlayerDashAttackState(PlayerMovement m, PlayerCombat c) : base(m, c) {}

    public override void Enter()
    {
        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Dash Attack");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        playerCombat.ResetDashStateExitTime();
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}