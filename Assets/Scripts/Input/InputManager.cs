using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    static public PlayerInput PlayerInput;
    static public Vector2 Movement;
    static public bool JumpWasPressed;
    static public bool JumpIsHeld;
    static public bool JumpWasReleased;
    static public bool DashWasPressed;
    static public bool ParryWasPressed;
    static public bool ParryIsHeld;
    static public bool ParryWasReleased;
    static public bool AttackWasPressed;
    static public bool HeavyAttackWasPressed;
    static public bool HeavyAttackIsHeld;
    static public bool HeavyAttackWasReleased;
    static public bool HealWasPressed;
    static public bool ToggleMaskWasPressed;
    static public bool ToggleMaskWasReleased;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _parryAction;
    private InputAction _heavyAttackAction;
    private InputAction _attackAction;
    private InputAction _healAction;
    private InputAction _toggleMaskAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _dashAction = PlayerInput.actions["Dash"];
        _parryAction = PlayerInput.actions["Parry"];
        _heavyAttackAction = PlayerInput.actions["Heavy Attack"];
        _attackAction = PlayerInput.actions["Attack"];
        _healAction = PlayerInput.actions["Heal"];
        _toggleMaskAction = PlayerInput.actions["Toggle Mask"];
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        JumpWasPressed = _jumpAction.WasPerformedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        DashWasPressed = _dashAction.WasPerformedThisFrame();
        AttackWasPressed = _attackAction.WasPerformedThisFrame();
        ParryWasPressed = _parryAction.WasPerformedThisFrame();
        ParryIsHeld = _parryAction.IsPressed();
        ParryWasReleased = _parryAction.WasReleasedThisFrame();
        HeavyAttackWasPressed = _heavyAttackAction.WasPerformedThisFrame();
        HeavyAttackIsHeld = _heavyAttackAction.IsPressed();
        HeavyAttackWasReleased = _heavyAttackAction.WasReleasedThisFrame();
        HealWasPressed = _healAction.WasPerformedThisFrame();
        ToggleMaskWasPressed = _toggleMaskAction.WasPerformedThisFrame();
        ToggleMaskWasReleased = _toggleMaskAction.WasReleasedThisFrame();
    }
}
