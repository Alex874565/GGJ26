using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerCombatStats", menuName = "ScriptableObjects/PlayerCombatStats")]
public class PlayerCombatStats : ScriptableObject
{
    [Header("Combo Attack Stats")]
    public float MaxTimeBetweenAttacks = 1.0f;
    public float BetweenAttackCooldown = 0f;
    public List<AttackData> ComboAttacksData;

    [Header("Heavy Attack Stats")]
    public float HeavyAttackDamage = 10.0f;

    [Header("Counter Attack Stats")]
    public float CounterAttackDamage = 5.0f;

    [Header("Dash Stats")]
    public float DashDistance = 5.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1.0f;

    [Header("Dash Attack Stats")]
    public AttackData DashAttackData;
    
    

    [Header("Buffer Times")]
    public float HeavyAttackBufferTime = 0.2f;
    public float ComboAttackBufferTime = 0.1f;
    public float ParryBufferTime = 0.1f;
    public float CounterAttackBufferTime = 0.1f;
    public float DashBufferTime = 0.1f;
}