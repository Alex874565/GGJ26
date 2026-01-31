using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent healthDepleted;
    public UnityEvent healthChanged;
    public float currentHealth;
    public float maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    void TakeDamage(float damage)
    {
        currentHealth = Math.Clamp(currentHealth - damage, 0, maxHealth);
        healthChanged.Invoke();
        if (currentHealth == 0)
            healthDepleted.Invoke();
    }

    void Heal(float healedAmount)
    {
        currentHealth = Math.Clamp(currentHealth + healedAmount, 0, maxHealth);
        healthChanged.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
