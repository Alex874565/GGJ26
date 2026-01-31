using UnityEngine;

public class PlayerHeavyAttackState : PlayerState
{
    public bool IsBeingHeld;
    private float _chargeTime;
    private float _chargeTimer;
    public bool IsAttacking;
    public float TimeSinceExit;

    public PlayerHeavyAttackState(PlayerMovement _playerMovement, PlayerCombat _playerCombat) : base(_playerMovement, _playerCombat)
    {
        TimeSinceExit = 0f;
        _chargeTime = playerCombat.PlayerCombatStats.HeavyAttackChargeTime;
    }
    public override void Enter()
    {
        playerMovement.Rb.linearVelocity = Vector3.zero;
        _chargeTimer = 0;
        playerCombat.Animator.SetTrigger("Charge Heavy Attack");
    }

    public override void Update()
    {
        if (!IsAttacking)
        {
            if (!IsBeingHeld)
            {
                playerCombat.Animator.SetTrigger("Leave Charge");
                playerCombat.ExitState();
                return;
            }
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer >= _chargeTime)
            {
                IsAttacking = true;
                playerCombat.Animator.SetBool("IsAttacking", true);
            }
        }
    }

    public override void Exit()
    {
        if (IsAttacking)
        {
            TimeSinceExit = 0f;
        }

        IsBeingHeld = false;
        IsAttacking = false;
        playerCombat.Animator.SetBool("IsAttacking", false);
    }
}