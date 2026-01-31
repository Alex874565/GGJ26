using UnityEngine;

/// <summary>
/// Handles visual telegraph (icon) for boss attacks.
/// Shows an icon above the boss to indicate attack type.
/// Animation is handled externally (e.g., via Animator).
/// </summary>
public class BossTelegraph : MonoBehaviour
{
    [Header("Telegraph Icon")]
    [SerializeField] private GameObject _parryIcon;
    [SerializeField] private GameObject _dodgeIcon;
    [SerializeField] private GameObject _bothIcon;

    private GameObject _currentIcon;

    private void Awake()
    {
        HideAllIcons();
    }

    /// <summary>
    /// Show the telegraph icon for the given attack type.
    /// </summary>
    public void StartTelegraph(AttackType attackType, float duration)
    {
        HideAllIcons();
        _currentIcon = GetIcon(attackType);
        if (_currentIcon != null)
            _currentIcon.SetActive(true);
    }

    /// <summary>
    /// Hide the telegraph icon.
    /// </summary>
    public void StopTelegraph()
    {
        HideAllIcons();
    }

    private void HideAllIcons()
    {
        if (_parryIcon != null) _parryIcon.SetActive(false);
        if (_dodgeIcon != null) _dodgeIcon.SetActive(false);
        if (_bothIcon != null) _bothIcon.SetActive(false);
        _currentIcon = null;
    }

    private GameObject GetIcon(AttackType attackType)
    {
        return attackType switch
        {
            AttackType.ParryOnly => _parryIcon,
            AttackType.DodgeOnly => _dodgeIcon,
            _ => _bothIcon
        };
    }
}
