using UnityEngine;

public class BossDashState : BossCombatState
{
    public float TimeSinceExit;
    public float Cooldown;
    public bool WillDashAttack; // Decided on enter
    private float _timeSinceDashStarted;
    private float _dashVelocity;
    private float _dashDirection;

    public BossDashState(BossMovement movement, BossCombat combat) : base(movement, combat)
    {
        TimeSinceExit = 999f; // Start high so dash attack doesn't trigger falsely at game start
        Cooldown = combat.BossCombatStats.DashCooldown;
    }

    public override void Enter()
    {
        _timeSinceDashStarted = 0f;
        
        // Decide now if this dash will follow up with an attack
        WillDashAttack = Random.value <= bossCombat.BossCombatStats.DashAttackChance;

        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsDashing", true);

        float dir = bossMovement.IsFacingRight ? 1f : -1f;
        _dashVelocity = bossCombat.BossCombatStats.DashDistance / bossCombat.BossCombatStats.DashDuration;
        _dashDirection = dir;
        
        // Take control of velocity
        bossMovement.ExternalVelocityControl = true;
    }

    public override void Update()
    {
        _timeSinceDashStarted += Time.deltaTime;
        if (_timeSinceDashStarted >= bossCombat.BossCombatStats.DashDuration)
        {
            // Dash finished - either attack or recharge
            if (WillDashAttack)
            {
                bossCombat.TransitionToDashAttack();
            }
            else
            {
                bossCombat.ExitCombatState();
            }
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
