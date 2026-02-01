using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossCombatStats", menuName = "ScriptableObjects/BossCombatStats")]
public class BossCombatStats : ScriptableObject
{
    [Header("Combo Attack")]
    public float MaxTimeBetweenAttacks = 1.0f;
    public float BetweenAttackCooldown = 0f;
    public float ComboAttackCooldown = 0.5f;
    public List<AttackData> ComboAttacksData;

    [Header("Heavy Attack")]
    public AttackData HeavyAttackData;
    public float HeavyAttackChargeTime = 1.0f;
    public float HeavyAttackCooldown = 2.0f;

    [Header("Dash")]
    public float DashDistance = 5.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1.0f;

    [Header("Dash Attack")]
    public float AfterDashAttackDelay = 0.5f;
    public AttackData DashAttackData;

    [Header("Parry")]
    public float ParryDuration = 0.5f;
    public float ParryCooldown = 2.0f;
    public float ParryRange = 3f;
    [Range(0f, 1f), Tooltip("Chance to parry when player attacks (0 = never, 1 = always)")]
    public float ParryChance = 0.5f;

    [Header("Range Thresholds")]
    public float ComboAttackRange = 3f;
    public float HeavyAttackRange = 4f;
    public float DashRange = 8f;

    [Header("Timing")]
    public float MinIdleTimeBeforeAttack = 0.3f;
    public float MaxIdleTimeBeforeAttack = 1.5f;

    [Header("Attack Weights (higher = more likely)")]
    [Range(0f, 10f)] public float ComboWeight = 5f;
    [Range(0f, 10f)] public float HeavyWeight = 3f;
    [Range(0f, 10f)] public float DashWeight = 4f;

    [Header("Phase 2 (when health below threshold)")]
    [Range(0f, 1f)] public float Phase2HealthThreshold = 0.5f;
    [Range(0.5f, 2f)] public float Phase2SpeedMultiplier = 1.3f;
    [Range(0.5f, 2f)] public float Phase2AggressionMultiplier = 1.5f;
}
