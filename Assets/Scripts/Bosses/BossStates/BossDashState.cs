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
        bossMovement.Rb.linearVelocity = Vector2.zero;
        _timeSinceDashStarted = 0f;

        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsDashing", true);

        float dir = bossMovement.IsFacingRight ? 1f : -1f;
        float dashVelocity = bossCombat.BossCombatStats.DashDistance / bossCombat.BossCombatStats.DashDuration;
        bossMovement.Rb.linearVelocity = new Vector2(dir * dashVelocity, 0f);
    }

    public override void Update()
    {
        _timeSinceDashStarted += Time.deltaTime;
        if (_timeSinceDashStarted >= bossCombat.BossCombatStats.DashDuration)
        {
            bossCombat.ExitCombatState();
        }
    }

    public override void Exit()
    {
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetBool("IsDashing", false);
        TimeSinceExit = 0f;
        bossMovement.Rb.linearVelocity = Vector2.zero;
    }
}
