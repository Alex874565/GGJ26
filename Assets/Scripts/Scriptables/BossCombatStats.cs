using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossCombatStats", menuName = "ScriptableObjects/BossCombatStats")]
public class BossCombatStats : ScriptableObject
{
    [Header("Hit Particles")]
    public GameObject DefaultHitParticlePrefab;
    [Tooltip("Scale of spawned particles (1 = normal)")]
    public float DefaultHitParticleSize = 0.5f;
    [Tooltip("Time before particle instance is destroyed")]
    public float DefaultHitParticleDestroyDelay = 3f;

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
    [Range(0f, 1f), Tooltip("Chance to follow up dash with an attack (0 = never, 1 = always)")]
    public float DashAttackChance = 0.6f;
    public AttackData DashAttackData;

    [Header("Parry")]
    public float ParryDuration = 0.5f;
    public float ParryCooldown = 3f;
    [Tooltip("Boss only parries when player is within this range")]
    public float ParryRange = 2f;
    [Range(0f, 1f), Tooltip("Chance to parry when player attacks (0 = never, 1 = always)")]
    public float ParryChance = 0.25f;
    [Tooltip("Knockback distance when boss parries player attack")]
    public float ParryKnockbackDistance = 0.8f;
    public float ParryKnockbackDuration = 0.1f;
    [Tooltip("Duration boss is immune to stun after parrying")]
    public float ParryStunImmunityDuration = 0.3f;

    [Header("Blocked (when player parries boss attack)")]
    public float BlockedDashBackDistance = 1.5f;
    public float BlockedDashBackDuration = 0.15f;

    [Header("Range Thresholds")]
    public float ComboAttackRange = 3f;
    public float HeavyAttackRange = 4f;
    public float DashRange = 8f;
    [Tooltip("Boss won't dash when player is closer than this")]
    public float MinDashRange = 2.5f;

    [Header("Recharge (after attack)")]
    [Tooltip("Minimum time boss waits after an attack (recharge animation plays)")]
    public float MinRechargeTime = 0.5f;
    [Tooltip("Maximum time boss waits after an attack (recharge animation plays)")]
    public float MaxRechargeTime = 1.5f;
    [Tooltip("Time boss stands still after recharge animation ends before moving")]
    public float PostAnimationDelay = 0.3f;
    [Tooltip("Time boss can move/reposition before attacking")]
    public float RepositionTime = 0.5f;

    [Header("Attack Weights (higher = more likely)")]
    [Range(0f, 10f)] public float ComboWeight = 5f;
    [Range(0f, 10f)] public float HeavyWeight = 3f;
    [Range(0f, 10f)] public float DashWeight = 4f;

    [Header("Phase 2 (when health below threshold)")]
    [Range(0f, 1f)] public float Phase2HealthThreshold = 0.5f;
    [Range(0.5f, 2f)] public float Phase2SpeedMultiplier = 1.3f;
    [Range(0.5f, 2f)] public float Phase2AggressionMultiplier = 1.5f;

    [Header("Stun State")]
    public float StunDuration = 1.5f;
    public float StunGracePeriod = 2.0f;

    public void SpawnHitParticles(Vector3 position)
    {
        if (DefaultHitParticlePrefab == null) return;
        var instance = Object.Instantiate(DefaultHitParticlePrefab, position, Quaternion.identity);
        instance.name = "HitParticles_" + DefaultHitParticlePrefab.name;
        instance.transform.localScale = Vector3.one * DefaultHitParticleSize;
        // Ensure particles render in front of sprites
        foreach (var r in instance.GetComponentsInChildren<Renderer>())
            r.sortingOrder = 32767;
        Object.Destroy(instance, DefaultHitParticleDestroyDelay);
    }
}
