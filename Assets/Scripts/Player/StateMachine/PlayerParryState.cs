using UnityEngine;

public class PlayerParryState : PlayerState
{
    public bool IsBeingHeld;
    public bool ParryRaised;

    public PlayerParryState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
    }

    public override void Enter()
    {
        playerMovement.Rb.linearVelocityX = 0;
        playerCombat.PlayerManager.OnParry?.Invoke();
        playerCombat.Animator.SetTrigger("Raise Parry");
        ParryRaised = true;
    }
    
    public void LowerParry()
    {
        ParryRaised = false;
        Debug.Log("Lowering Parry");
        playerCombat.Animator.SetTrigger("Lower Parry");
    }

    public override void Exit()
    {
        
    }
}