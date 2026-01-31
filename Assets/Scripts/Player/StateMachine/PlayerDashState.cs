using UnityEngine;

public class PlayerDashState : PlayerState
{
    public float BufferTimer;
    public float Cooldown;
    public float TimeSinceExit;

    private float timeSinceDashStarted;

    public PlayerDashState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
        Cooldown = playerCombat.PlayerCombatStats.DashCooldown;
        TimeSinceExit = 0f;
    }

    public override void Enter()
    {
        playerMovement.Rb.linearVelocity = Vector2.zero;
        TimeSinceExit = -999999999f;
        timeSinceDashStarted = 0f;
        playerCombat.Animator.SetBool("IsDashing", true);

        float direction = playerMovement.IsFacingRight ? 1f : -1f;
        float dashTime = playerCombat.PlayerCombatStats.DashDuration;
        float distance = playerCombat.PlayerCombatStats.DashDistance;

        // Set velocity directly - works regardless of mass
        float dashVelocity = distance / dashTime;
        playerMovement.Rb.linearVelocity = new Vector2(direction * dashVelocity, 0f);
    }

    public override void FixedUpdate()
    {
        timeSinceDashStarted += Time.fixedDeltaTime;
        if (timeSinceDashStarted >= playerCombat.PlayerCombatStats.DashDuration)
        {
            playerCombat.ExitState();
        }
    }

    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsDashing", false);
        TimeSinceExit = 0f;
        // Optional: reduce velocity to prevent sliding
        playerMovement.Rb.linearVelocity *= 0.5f;
    }
}