using UnityEngine;

public interface IDamageable
{
    void TakeHit(int damage);
    
    void TakeKnockback(float force, Vector2 direction, float stunChance);
}