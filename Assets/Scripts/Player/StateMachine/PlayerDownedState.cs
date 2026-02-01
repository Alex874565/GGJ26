using UnityEngine;

public class PlayerDownedState : PlayerState
{
    private float _downedDuration;
    private float _getUpDuration;
    private float _gracePeriod;
    private float _timer;
    private float _timeSinceLastDowned;
    private bool _getUpTriggered;

    public bool IsInGracePeriod => _timeSinceLastDowned < _gracePeriod;

    public PlayerDownedState(PlayerMovement movement, PlayerCombat combat, float duration = 1.0f, float getUpDuration = 0.5f, float gracePeriod = 2.0f) 
        : base(movement, combat)
    {
        _downedDuration = duration;
        _getUpDuration = getUpDuration;
        _gracePeriod = gracePeriod;
        _timeSinceLastDowned = gracePeriod + 1f;
    }

    public void UpdateGraceTimer(float deltaTime)
    {
        _timeSinceLastDowned += deltaTime;
    }

    public override void Enter()
    {
        _timer = 0f;
        _getUpTriggered = false;
        
        playerMovement.Rb.linearVelocity = Vector2.zero;
        playerCombat.StopAllCoroutines();
        
        playerCombat.Animator.SetTrigger("Downed");
        playerCombat.Animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= _downedDuration && !_getUpTriggered)
        {
            _getUpTriggered = true;
            playerCombat.Animator.SetTrigger("Get Up");
        }
        
        if (_timer >= _downedDuration + _getUpDuration)
        {
            playerCombat.ExitState();
        }
    }

    public override void FixedUpdate()
    {
        playerMovement.Rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        _timeSinceLastDowned = 0f;
    }
}
