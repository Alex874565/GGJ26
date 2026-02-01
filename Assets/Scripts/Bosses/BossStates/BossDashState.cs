using UnityEngine;

public class BossDashState : BossCombatState
{
    public float TimeSinceExit;
    public float Cooldown;
    private float _timeSinceDashStarted;

    public BossDashState(BossMovement movement, BossCombat combat) : base(movement, combat)
    {
        TimeSinceExit = 0f;
        Cooldown = combat.BossCombatStats.DashCooldown;
    }

    public override void Enter()
    {
        _timeSinceDashStarted = 0f;

        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsDashing", true);

        float dir = bossMovement.IsFacingRight ? 1f : -1f;
        _dashVelocity = bossCombat.BossCombatStats.DashDistance / bossCombat.BossCombatStats.DashDuration;
        _dashDirection = dir;
        
        // Take control of velocity
        bossMovement.ExternalVelocityControl = true;
    }

    private float _dashVelocity;
    private float _dashDirection;

    public override void Update()
    {
        _timeSinceDashStarted += Time.deltaTime;
        if (_timeSinceDashStarted >= bossCombat.BossCombatStats.DashDuration)
        {
            bossCombat.ExitCombatState();
        }
    }

    public override void FixedUpdate()
    {
        // Apply dash velocity directly to rigidbody
        bossMovement.Rb.linearVelocity = new Vector2(_dashDirection * _dashVelocity, 0f);
    }

    public override void Exit()
    {
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsDashing", false);
        TimeSinceExit = 0f;
        
        // Return control to BossMovement
        bossMovement.ExternalVelocityControl = false;
        bossMovement.Rb.linearVelocity = new Vector2(0f, bossMovement.Rb.linearVelocity.y);
    }
}
