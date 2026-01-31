using UnityEngine;

public class BossCombatIdleState : BossCombatState
{
    public float TimeInIdle;

    public BossCombatIdleState(BossMovement movement, BossCombat combat) : base(movement, combat) { }

    public override void Enter()
    {
        TimeInIdle = 0f;
    }

    public override void Update()
    {
        TimeInIdle += Time.deltaTime;
    }
}
