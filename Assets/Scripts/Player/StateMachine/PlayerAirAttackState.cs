
public class PlayerAirAttackState : PlayerState
{
    public PlayerAirAttackState(PlayerMovement m, PlayerCombat c) : base(m, c)
    {
        TimeSinceExit = 0f;
    }

    public float BufferTimer;
    public float TimeSinceExit;
    
    public override void Enter()
    {
        playerCombat.Animator.SetBool("IsAttacking", true);
        playerCombat.Animator.SetTrigger("Air Attack");
    }
    
    public override void Update()
    {
        if (playerMovement.IsGrounded)
        {
            //playerCombat.Animator.SetTrigger("Land");
            //playerCombat.ExitState();
        }
    }
    
    public override void Exit()
    {
        TimeSinceExit = 0;
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
    
}
