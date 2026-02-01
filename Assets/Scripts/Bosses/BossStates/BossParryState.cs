using UnityEngine;

public class BossParryState : BossCombatState
{
    public bool ParryRaised;
    public float TimeSinceExit;
    private float _parryTimer;

    public BossParryState(BossMovement movement, BossCombat combat) : base(movement, combat)
    {
        TimeSinceExit = 0f;
    }

    public override void Enter()
    {
        bossMovement.SetHorizontalVelocity(0f);
        ParryRaised = true;
        _parryTimer = 0f;
        
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetTrigger("Raise Parry");
    }

    public override void Update()
    {
        _parryTimer += Time.deltaTime;
        
        // Auto-lower parry after duration
        if (_parryTimer >= bossCombat.BossCombatStats.ParryDuration)
        {
            LowerParry();
        }
    }

    public void LowerParry()
    {
        if (!ParryRaised) return;
        
        ParryRaised = false;
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetTrigger("Lower Parry");
        
        bossCombat.ExitCombatState();
    }

    public override void Exit()
    {
        TimeSinceExit = 0f;
        ParryRaised = false;
    }
}
