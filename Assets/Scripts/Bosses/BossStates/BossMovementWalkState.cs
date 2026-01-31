using UnityEngine;

public class BossMovementWalkState : BossMovementState
{
    public BossMovementWalkState(BossMovement movement) : base(movement) { }

    public override void FixedUpdate()
    {
        bossMovement.MoveTowardTarget();
    }
}
