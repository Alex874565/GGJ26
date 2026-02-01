using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour, IDamageable
{
    #region Events

        public UnityAction OnMove;
        public UnityAction OnJump;
        public UnityAction OnDoubleJump;
        public UnityAction OnDash;
        public UnityAction OnAttack;
        public UnityAction OnHeavyAttack;
        public UnityAction OnComboAttack;
        public UnityAction OnDashAttack;
        public UnityAction OnJumpAttack;
        public UnityAction OnParry;

    #endregion

    private PlayerCombat _playerCombat;
    private PlayerMovement _playerMovement;

    private int _health;

    private void Awake()
    {
        _playerCombat = GetComponent<PlayerCombat>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _health = _playerCombat.PlayerCombatStats.Health;
    }

    public void TakeHit(int damage)
    {

    }

    public void TakeKnockback(float force, Vector2 direction)
    {

    }


}