public abstract class BossMovementState
{
    protected BossMovement bossMovement;

    public BossMovementState(BossMovement movement)
    {
        bossMovement = movement;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
