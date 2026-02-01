using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerCombatStats", menuName = "ScriptableObjects/PlayerCombatStats")]
public class PlayerCombatStats : ScriptableObject
{
    [Header("Hit Particles")]
    public GameObject DefaultHitParticlePrefab;
    [Tooltip("Scale of spawned particles (1 = normal)")]
    public float DefaultHitParticleSize = 0.5f;
    [Tooltip("Time before particle instance is destroyed")]
    public float DefaultHitParticleDestroyDelay = 3f;

    public int Health = 100;

    public float BufferTime = .1f;
    
    [Header("Combo Attack Stats")]
    public float MaxTimeBetweenAttacks = 1.0f;
    public float BetweenAttackCooldown = 0f;
    public float ComboAttackCooldown = 0.5f;
    public List<AttackData> ComboAttacksData;

    [Header("Air Attack Stats")]
    public AttackData AirAttackData;
    public float AirAttackCooldown = 1.0f;
    
    [Header("Heavy Attack Stats")]
    public AttackData HeavyAttackData;
    public float HeavyAttackChargeTime = 1.0f;
    public float HeavyAttackCooldown = 2.0f;

    [Header("Counter Attack Stats")]
    public AttackData CounterAttackData;
    public float CounterAttackWindow = 0.5f;

    [Header("Dash Stats")]
    public float DashDistance = 5.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1.0f;
    [Range(0f, 1f)] public float DashEaseInRatio = 0.2f;  // Portion of dash for acceleration
    [Range(0f, 1f)] public float DashEaseOutRatio = 0.3f; // Portion of dash for deceleration

    [Header("Dash Attack Stats")]
    public float AfterDashAttackDelay = .5f;
    public AttackData DashAttackData;

    [Header("Downed State")]
    public float DownedDuration = 1.0f;
    public float GetUpDuration = 0.5f;
    public float DownedGracePeriod = 2.0f;

    [Header("Healing")]
    public int HealAmount = 25;
    public float HealDuration = 1.0f;
    public int StartingPotions = 3;

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