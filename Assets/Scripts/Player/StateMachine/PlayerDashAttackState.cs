public class PlayerDashAttackState : PlayerState
{
    public float BufferTimer;

    private AttackData _attackData => playerCombat.PlayerCombatStats.DashAttackData;

    public PlayerDashAttackState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
    }

    public override void Enter()
    {
        playerCombat.Animator.SetTrigger("Dash Attack");
        playerCombat.Animator.SetBool("IsAttacking", true);
    }
    
    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}