using UnityEngine;

public class BossStunState : BossCombatState
{
    private float _stunDuration;
    private float _gracePeriod;
    private float _timer;
    private float _timeSinceLastStun;

    public bool IsInGracePeriod => _timeSinceLastStun < _gracePeriod;

    public BossStunState(BossMovement movement, BossCombat combat, float duration = 1.5f, float gracePeriod = 2.0f) 
        : base(movement, combat)
    {
        _stunDuration = duration;
        _gracePeriod = gracePeriod;
        _timeSinceLastStun = gracePeriod + 1f; // Start outside grace period
    }

    public void UpdateGraceTimer(float deltaTime)
    {
        _timeSinceLastStun += deltaTime;
    }

    public override void Enter()
    {
        _timer = 0f;
        
        // Stop all movement
        bossMovement.Rb.linearVelocity = Vector2.zero;
        
        // Stop any running attack coroutines
        bossCombat.StopAllCoroutines();
        
        // Trigger stun animation
        bossCombat.Animator.SetTrigger("Stunned");
        bossCombat.Animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        
        // Exit after duration
        if (_timer >= _stunDuration)
        {
            bossCombat.ExitToIdle();
        }
    }

    public override void FixedUpdate()
    {
        // Keep velocity at zero - no movement allowed
        bossMovement.Rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        // Start grace period
        _timeSinceLastStun = 0f;
    }
}
