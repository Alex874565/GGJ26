using UnityEngine;

[CreateAssetMenu(fileName = "BossMovementStats", menuName = "ScriptableObjects/BossMovementStats")]
public class BossMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 200f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;

    [Header("Grounded / Collision")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
}
