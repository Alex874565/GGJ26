using UnityEngine;

[RequireComponent(typeof(BossMovement), typeof(BossCombat))]
public class Boss : MonoBehaviour
{
    [SerializeField] private Health _health;
    [Header("Target (assign or leave null to use Player from ServiceLocator)")]
    [SerializeField] private Transform _playerTarget;

    private Animator _animator;
    private BossMovement _bossMovement;
    private BossCombat _bossCombat;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bossMovement = GetComponent<BossMovement>();
        _bossCombat = GetComponent<BossCombat>();
        if (_health == null)
            _health = GetComponent<Health>();
    }

    private void Start()
    {
        if (_playerTarget == null && ServiceLocator.Instance != null && ServiceLocator.Instance.PlayerManager != null)
        {
            _playerTarget = ServiceLocator.Instance.PlayerManager.transform;
        }

        if (_playerTarget != null)
        {
            _bossCombat.SetPlayerTarget(_playerTarget);
        }

        if (_health != null)
        {
            _health.healthDepleted.AddListener(OnHealthDepleted);
        }
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.healthDepleted.RemoveListener(OnHealthDepleted);
        }
    }

    private void OnHealthDepleted()
    {
        _bossCombat.EnterDeadState();
    }
}
