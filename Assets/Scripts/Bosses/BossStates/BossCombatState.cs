public abstract class BossCombatState
{
    protected BossMovement bossMovement;
    protected BossCombat bossCombat;

    public BossCombatState(BossMovement movement, BossCombat combat)
    {
        bossMovement = movement;
        bossCombat = combat;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
