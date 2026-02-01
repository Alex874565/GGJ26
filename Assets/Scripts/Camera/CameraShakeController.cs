using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Triggers camera shake via Cinemachine Impulse when attacks hit.
/// Add CinemachineImpulseSource to this GameObject and CinemachineImpulseListener to your Virtual Camera.
/// </summary>
[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShakeController : MonoBehaviour
{
    public static CameraShakeController Instance => _instance;
    private static CameraShakeController _instance;

    [SerializeField] private bool _enabled = true;
    [Tooltip("Multiplier to convert damage value to shake force. 1 damage = this amount of force.")]
    [SerializeField] private float _damageToForceMultiplier = 0.1f;
    [Tooltip("Base force when using Shake() without damage scaling.")]
    [SerializeField] private float _defaultForce = 1f;

    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    /// <summary>
    /// Shake with a specific force. 1 = normal strength.
    /// </summary>
    public void Shake(float force = -1f)
    {
        if (!_enabled || _impulseSource == null) return;
        float f = force >= 0 ? force : _defaultForce;
        _impulseSource.GenerateImpulseWithForce(f);
    }

    /// <summary>
    /// Shake scaled by damage amount. Use when an attack hits.
    /// </summary>
    public void ShakeFromDamage(int damage)
    {
        if (!_enabled || _impulseSource == null) return;
        float force = damage * _damageToForceMultiplier;
        if (force > 0)
            _impulseSource.GenerateImpulseWithForce(force);
    }

    /// <summary>
    /// Shake at a specific world position (e.g. impact point).
    /// </summary>
    public void ShakeAt(Vector3 position, float force = -1f)
    {
        if (!_enabled || _impulseSource == null) return;
        float f = force >= 0 ? force : _defaultForce;
        Vector3 velocity = Vector3.down * f;
        _impulseSource.GenerateImpulseAtPositionWithVelocity(position, velocity);
    }

    public void SetEnabled(bool enabled) => _enabled = enabled;
}
