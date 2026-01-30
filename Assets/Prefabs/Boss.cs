using System;
using UnityEngine;

public interface State
{
    void OnEnter();
    void Update();
    void FixedUpdate();
    void OnExit();
}    


public enum bossStates
{
    IDLE,
    RUN,
    DASH,
    PARRY,
    ATTACK,
    TAKE_DAMAGE,
    DEAD
}
public enum bossAttackTypes
{
    DODGEABLE_ATTACK,
    PARRYABLE_ATTACK
}

public class Boss : MonoBehaviour
{
    public Animator animator;
    public Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
