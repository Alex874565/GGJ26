using UnityEngine;

public class PlayerDashAttackState : PlayerState
{
    public float BufferTimer;

    private AttackData _attackData => playerCombat.PlayerCombatStats.DashAttackData;

    public PlayerDashAttackState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
    }

    public override void Enter()
    {
        Debug.Log(playerCombat.Animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Dash Attack");
    }
    
    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}