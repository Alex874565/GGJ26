using UnityEngine;

public class PlayerDownedState : PlayerState
{
    private float _downedDuration;
    private float _gracePeriod;
    private float _timer;
    private float _timeSinceLastDowned;

    public bool IsInGracePeriod => _timeSinceLastDowned < _gracePeriod;

    public PlayerDownedState(PlayerMovement movement, PlayerCombat combat, float duration = 1.0f, float gracePeriod = 2.0f) 
        : base(movement, combat)
    {
        _downedDuration = duration;
        _gracePeriod = gracePeriod;
        _timeSinceLastDowned = gracePeriod + 1f; // Start outside grace period
    }

    public void UpdateGraceTimer(float deltaTime)
    {
        _timeSinceLastDowned += deltaTime;
    }

    public override void Enter()
    {
        _timer = 0f;
        
        // Stop all movement
        playerMovement.Rb.linearVelocity = Vector2.zero;
        
        // Stop any running attack coroutines
        playerCombat.StopAllCoroutines();
        
        // Trigger downed animation
        playerCombat.Animator.SetTrigger("Downed");
        playerCombat.Animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        
        // Exit after duration
        if (_timer >= _downedDuration)
        {
            playerCombat.ExitState();
        }
    }

    public override void FixedUpdate()
    {
        // Keep velocity at zero - no movement allowed
        playerMovement.Rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        // Start grace period
        _timeSinceLastDowned = 0f;
        
        // Trigger get up animation
        playerCombat.Animator.SetTrigger("Get Up");
    }
}
