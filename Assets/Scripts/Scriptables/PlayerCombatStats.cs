using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerCombatStats", menuName = "ScriptableObjects/PlayerCombatStats")]
public class PlayerCombatStats : ScriptableObject
{
    [Header("Combo Attack Stats")]
    public float TimeBetweenComboAttacks = 1.0f;
    public List<float> ComboAttackDamages = new List<float>();

    [Header("Heavy Attack Stats")]
    public float HeavyAttackDamage = 10.0f;

    [Header("Counter Attack Stats")]
    public float CounterAttackDamage = 5.0f;

    [Header("Dash Stats")]
    public float DashDistance = 5.0f;

    [Header("Dash Attack Stats")]
    public float DashAttackDamage = 15.0f;

    [Header("Buffer Times")]
    public float HeavyAttackBufferTime = 0.2f;
    public float ComboAttackBufferTime = 0.1f;
    public float ParryBufferTime = 0.1f;
    public float CounterAttackBufferTime = 0.1f;
    public float DashBufferTime = 0.1f;
}