using UnityEngine;

public class PlayerDashState : PlayerState
{
    public float BufferTimer;
    public float Cooldown;
    public float TimeSinceExit;

    private float _timeSinceDashStarted;
    private float _dashDirection;
    private float _maxDashVelocity;
    private float _dashDuration;
    private float _easeInDuration;
    private float _easeOutStart;

    public PlayerDashState(PlayerMovement movement, PlayerCombat combat) 
        : base(movement, combat)
    {
        TimeSinceExit = 0f;
        Cooldown = combat.PlayerCombatStats.DashCooldown;
    }

    public override void Enter()
    {
        // Stop any running dash coroutines from attacks
        playerCombat.StopAllCoroutines();
        
        _timeSinceDashStarted = 0f;

        playerCombat.PlayerManager.OnDash?.Invoke();
        playerCombat.Animator.SetBool("IsDashing", true);

        _dashDirection = playerMovement.IsFacingRight ? 1f : -1f;
        _dashDuration = playerCombat.PlayerCombatStats.DashDuration;
        
        // Calculate max velocity needed to cover the distance with easing
        // We need to account for the reduced distance during ease in/out
        float easeInRatio = playerCombat.PlayerCombatStats.DashEaseInRatio;
        float easeOutRatio = playerCombat.PlayerCombatStats.DashEaseOutRatio;
        _easeInDuration = _dashDuration * easeInRatio;
        _easeOutStart = _dashDuration * (1f - easeOutRatio);
        
        // Effective time at full speed (accounting for half-speed during easing)
        float effectiveTime = _dashDuration - (_easeInDuration * 0.5f) - ((_dashDuration - _easeOutStart) * 0.5f);
        _maxDashVelocity = playerCombat.PlayerCombatStats.DashDistance / effectiveTime;
    }

    public override void Update()
    {
        _timeSinceDashStarted += Time.deltaTime;

        if (_timeSinceDashStarted >= _dashDuration)
        {
            playerCombat.ExitState();
            return;
        }

        // Calculate current velocity with easing
        float currentVelocity = CalculateEasedVelocity();
        playerMovement.Rb.linearVelocity = new Vector2(_dashDirection * currentVelocity, 0f);
    }

    public override void FixedUpdate()
    {
        // Velocity is updated in Update for smoother response
    }

    private float CalculateEasedVelocity()
    {
        float t = _timeSinceDashStarted;
        
        // Ease in phase
        if (t < _easeInDuration && _easeInDuration > 0f)
        {
            float easeT = t / _easeInDuration;
            return _maxDashVelocity * EaseOutQuad(easeT);
        }
        // Ease out phase
        else if (t > _easeOutStart && _easeOutStart < _dashDuration)
        {
            float easeT = (t - _easeOutStart) / (_dashDuration - _easeOutStart);
            return _maxDashVelocity * (1f - EaseInQuad(easeT));
        }
        // Full speed phase
        return _maxDashVelocity;
    }

    // Quadratic ease out: fast start, slow end
    private float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    // Quadratic ease in: slow start, fast end
    private float EaseInQuad(float t)
    {
        return t * t;
    }

    public override void Exit()
    {
        playerCombat.Animator.SetBool("IsDashing", false);
        TimeSinceExit = 0f;
        
        // Don't modify velocity here - let the next state handle it
        // This prevents momentum loss when chaining into dash attack
    }
}