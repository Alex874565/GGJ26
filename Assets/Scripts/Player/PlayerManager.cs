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
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }

    }

    public void TakeKnockback(float force, Vector2 direction, float stunChance)
    {
        // Apply knockback force
        if (_playerMovement != null && force > 0)
        {
            _playerMovement.Rb.linearVelocity = direction * force;
        }

        Debug.Log($"Player Knocked Back with force {force} in direction {direction}");

        // Roll for stun (downed)
        if (stunChance > 0 && Random.value <= stunChance)
        {
            Debug.Log("Player Downed!");
            _playerCombat.EnterDownedState();
        }
    }

    public void Die()
    {
        // Handle player death (not implemented)
    }

    public void Heal(int amount)
    {
        int maxHealth = _playerCombat.PlayerCombatStats.Health;
        _health = Mathf.Min(_health + amount, maxHealth);
        Debug.Log($"Player healed. Current health: {_health}/{maxHealth}");
    }

    public int CurrentHealth => _health;
}