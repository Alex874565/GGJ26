using UnityEngine;

[CreateAssetMenu(fileName = "BossesStats", menuName = "Scriptable Objects/BossesStats")]
public class BossesStats : ScriptableObject
{
    [Header("Depression")]
    [Range(.25f, 50f)] public float MaxHealth = 20f;
    [Range(1f, 200f)] public float MaxWalkSpeed = 12.5f;
    [Range(.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(.25f, 50f)] public float DodgeableAttackDamage = 20f;
    [Range(.25f, 50f)] public float ParryableAttackDamage = 20f;
}
