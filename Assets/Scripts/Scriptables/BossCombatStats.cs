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

    [Header("Condition thresholds (AI)")]
    [Tooltip("Distance to player below which boss will try combo attack")]
    public float ComboAttackRange = 3f;
    [Tooltip("Distance to player below which boss will try heavy attack")]
    public float HeavyAttackRange = 4f;
    [Tooltip("Distance to player below which boss will try dash")]
    public float DashRange = 8f;
    [Tooltip("Min time in idle before boss can choose an attack again")]
    public float MinIdleTimeBeforeAttack = 0.3f;
}
