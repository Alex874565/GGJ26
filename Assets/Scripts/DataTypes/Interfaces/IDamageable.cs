using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
    
    void TakeKnockback(float force, Vector2 direction);
}