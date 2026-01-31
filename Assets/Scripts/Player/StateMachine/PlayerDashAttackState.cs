using UnityEngine;

public class PlayerDashAttackState : PlayerState
{
    public float BufferTimer;

    public PlayerDashAttackState(PlayerMovement m, PlayerCombat c) : base(m, c) {}

    public override void Enter()
    {
        // Stop any existing momentum - dash attack has its own movement
        playerMovement.Rb.linearVelocity = Vector2.zero;
        
        playerCombat.PlayerManager.OnAttack?.Invoke();
        playerCombat.PlayerManager.OnDashAttack?.Invoke();
        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Dash Attack");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        // Set TimeSinceExit past the AfterDashAttackDelay window so we don't chain dash attacks
        playerCombat.SetDashExitTimePastWindow();
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}