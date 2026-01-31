using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerCombatStats", menuName = "ScriptableObjects/PlayerCombatStats")]
public class PlayerCombatStats : ScriptableObject
{
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
    public float CounterAttackDamage = 5.0f;

    [Header("Dash Stats")]
    public float DashDistance = 5.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1.0f;
    [Range(0f, 1f)] public float DashEaseInRatio = 0.2f;  // Portion of dash for acceleration
    [Range(0f, 1f)] public float DashEaseOutRatio = 0.3f; // Portion of dash for deceleration

    [Header("Dash Attack Stats")]
    public float AfterDashAttackDelay = .5f;
    public AttackData DashAttackData;
}