using UnityEngine;

public class PlayerHealState : PlayerState
{
    private float _healDuration;
    private float _timer;
    private bool _healApplied;

    public PlayerHealState(PlayerMovement movement, PlayerCombat combat, float duration = 1.0f) 
        : base(movement, combat)
    {
        _healDuration = duration;
    }

    public override void Enter()
    {
        _timer = 0f;
        _healApplied = false;
        
        // Stop all movement
        playerMovement.Rb.linearVelocity = Vector2.zero;
        
        // Trigger heal animation
        playerCombat.Animator.SetTrigger("Heal");
        playerCombat.Animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        
        // Apply heal at midpoint of animation
        if (!_healApplied && _timer >= _healDuration * 0.5f)
        {
            _healApplied = true;
            playerCombat.ApplyHeal();
        }
        
        // Exit after duration
        if (_timer >= _healDuration)
        {
            playerCombat.ExitState();
        }
    }

    public override void FixedUpdate()
    {
        // Keep velocity at zero - no movement while healing
        playerMovement.Rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
    }
}
