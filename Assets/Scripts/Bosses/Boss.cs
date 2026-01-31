using System;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator animator;
    public Health health;

    private BossState _currentState;
    private BossIdleState _idleState;
    private BossWalkState _walkState;
    private BossDashState _dashState;
    private BossAttackState _attackState;
    private BossDeadState _deadState;

    public enum bossStates
    {
        Idle,
        Walk,
        Dash,
        Attack,
        Dead
    }
    public bossStates currentBossState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        _idleState = new BossIdleState();
        _walkState = new BossWalkState();
        _dashState = new BossDashState();
        _attackState = new BossAttackState();
        _deadState = new BossDeadState();

        _currentState = _idleState;
        currentBossState = bossStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckStateTransitions()
    {
        switch (currentBossState)
        {
            case bossStates.Idle:
                ChangeState(_idleState);
                break;

                
        }

        
    }

    private void ChangeState(BossState state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    public void ExitState()
    {
        ChangeState(_idleState);
    }



}
