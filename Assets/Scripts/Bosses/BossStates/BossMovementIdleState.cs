using UnityEngine;

public class BossMovementIdleState : BossMovementState
{
    public BossMovementIdleState(BossMovement movement) : base(movement) { }

    public override void Enter()
    {
        bossMovement.SetHorizontalVelocity(0f);
    }

    public override void FixedUpdate()
    {
        bossMovement.SetHorizontalVelocity(0f);
    }
}
