using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputReader : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;

    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string jumpActionName = "Jump";

    public Vector2 MoveValue { get; private set; }
    public bool SprintHeld { get; private set; }

    public event Action JumpPressed;
    public event Action JumpReleased;

    private InputActionMap map;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private void Awake()
    {
        if (actions == null) { enabled = false; return; }

        map = actions.FindActionMap(actionMapName, true);

        moveAction = map.FindAction(moveActionName, true);
        sprintAction = map.FindAction(sprintActionName, false);
        jumpAction = map.FindAction(jumpActionName, false);

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        if (sprintAction != null)
        {
            sprintAction.performed += OnSprint;
            sprintAction.canceled += OnSprint;
        }

        if (jumpAction != null)
        {
            jumpAction.performed += OnJump;
            jumpAction.canceled += OnJumpCanceled;
        }
    }

    private void OnEnable() => map?.Enable();
    private void OnDisable() => map?.Disable();

    private void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
        }

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprint;
            sprintAction.canceled -= OnSprint;
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
            jumpAction.canceled -= OnJumpCanceled;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx) => MoveValue = ctx.ReadValue<Vector2>();

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        bool held = ctx.ReadValueAsButton();
        if (SprintHeld == held) return;
        SprintHeld = held;
    }

    private void OnJump(InputAction.CallbackContext ctx) => JumpPressed?.Invoke();
    private void OnJumpCanceled(InputAction.CallbackContext ctx) => JumpReleased?.Invoke();
}
