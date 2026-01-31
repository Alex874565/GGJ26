using UnityEngine;

public class PlayerDashState : PlayerState
{
    public float BufferTimer;
    public float Cooldown;
    public float TimeSinceExit;

    private float _timeSinceDashStarted;

    public PlayerDashState(PlayerMovement movement, PlayerCombat combat) 
        : base(movement, combat)
    {
        TimeSinceExit = 0f;
        Cooldown = combat.PlayerCombatStats.DashCooldown;
    }

    public override void Enter()
    {
        playerMovement.Rb.linearVelocity = Vector2.zero;

        _timeSinceDashStarted = 0f;

        playerCombat.Animator.SetBool("IsDashing", true);

        float dir = playerMovement.IsFacingRight ? 1f : -1f;
        float dashVelocity =
            playerCombat.PlayerCombatStats.DashDistance /
            playerCombat.PlayerCombatStats.DashDuration;

        playerMovement.Rb.linearVelocity = new Vector2(dir * dashVelocity, 0f);
    }

    public override void Update()
    {
        _timeSinceDashStarted += Time.deltaTime;

        if (_timeSinceDashStarted >= playerCombat.PlayerCombatStats.DashDuration)
        {
            playerCombat.ExitState();
        }
    }

    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsDashing", false);
        TimeSinceExit = 0f;
        playerMovement.Rb.linearVelocity = Vector2.zero;
    }
}