using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerMovement movement, PlayerCombat combat) : base(movement, combat) { }

    public override void Enter()
    {
        playerMovement.Rb.linearVelocity = Vector2.zero;
        playerCombat.StopAllCoroutines();
        if (playerCombat.Animator != null)
            playerCombat.Animator.SetTrigger("Die");
    }

    public override void Update() { }
    public override void FixedUpdate()
    {
        playerMovement.Rb.linearVelocity = Vector2.zero;
    }
}
