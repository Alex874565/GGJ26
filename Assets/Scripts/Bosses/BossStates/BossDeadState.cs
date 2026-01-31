using UnityEngine;

public class BossDeadState : BossCombatState
{
    public BossDeadState(BossMovement movement, BossCombat combat) : base(movement, combat) { }

    public override void Enter()
    {
        bossMovement.Rb.linearVelocity = Vector2.zero;
        if (bossCombat.Animator != null)
            bossCombat.Animator.SetTrigger("Death");
    }

    public override void Update() { }
    public override void FixedUpdate() { }
}
