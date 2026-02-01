using UnityEngine;

[CreateAssetMenu(fileName = "BossMovementStats", menuName = "ScriptableObjects/BossMovementStats")]
public class BossMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 200f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Tooltip("Boss stops moving when this close to the player")]
    [Range(0.5f, 10f)] public float StoppingDistance = 2f;
    [Tooltip("Boss backs away if player gets closer than this")]
    [Range(0.5f, 10f)] public float MinimumDistance = 1.5f;

    [Header("Gravity")]
    public float Gravity = -50f;
    public float MaxFallSpeed = 26f;

    [Header("Grounded / Collision")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
}
