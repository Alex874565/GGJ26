using UnityEngine;

[System.Serializable]
public class AttackData
{
    public int Damage;
    public float DashDistance;
    public float DashDuration;
    public float KnockbackForce;
    
    [Header("Defense Type")]
    public AttackType AttackType = AttackType.Both;
    
    [Tooltip("Time before attack hits to show the telegraph flash")]
    public float TelegraphDuration = 0.5f;
}
