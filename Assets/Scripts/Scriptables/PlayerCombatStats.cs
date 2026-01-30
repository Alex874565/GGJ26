using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerCombatStats", menuName = "ScriptableObjects/PlayerCombatStats")]
public class PlayerCombatStats : ScriptableObject
{
    [Header("Combo Attack Stats")]
    public List<float> ComboAttackDamages;

    [Header("Heavy Attack Stats")]
    public float HeavyAttackDamage;

    [Header("Counter Attack Stats")]
    public float CounterAttackDamage;

    [Header("Dash Stats")]
    public float DashDistance;

    [Header("Dash Attack Stats")]
    public float DashAttackDamage;

    [Header("Buffer Times")]
    public float HeavyAttackBufferTime;
    public float ComboAttackBufferTime;
    public float ParryBufferTime;
    public float CounterAttackBufferTime;
    public float DashBufferTime;
}