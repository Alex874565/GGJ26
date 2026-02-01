public class PlayerCounterAttackState : PlayerState
{
    public float BufferTimer;
    public float LastSuccessfulParryTime;

    public PlayerCounterAttackState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
        LastSuccessfulParryTime = _playerCombat.PlayerCombatStats.CounterAttackWindow + 1f;
    }
    public override void Enter()
    {
        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Counter Attack");
    }
    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}