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

    // ✅ NEW (Tab)
    [SerializeField] private string toggleUIActionName = "ToggleUI";

    public Vector2 MoveValue { get; private set; }
    public bool SprintHeld { get; private set; }

    public event Action JumpPressed;
    public event Action JumpReleased;

    // ✅ NEW
    public event Action ToggleUI;

    private InputActionMap map;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    // ✅ NEW
    private InputAction toggleUIAction;

    private void Awake()
    {
        if (actions == null) { enabled = false; return; }

        map = actions.FindActionMap(actionMapName, true);

        moveAction = map.FindAction(moveActionName, true);
        sprintAction = map.FindAction(sprintActionName, false);
        jumpAction = map.FindAction(jumpActionName, false);

        // ✅ NEW
        toggleUIAction = map.FindAction(toggleUIActionName, false);

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
        else
        {
            Debug.LogWarning($"[InputReader] Jump action '{jumpActionName}' not found in map '{actionMapName}'");
        }

        // ✅ NEW
        if (toggleUIAction != null)
        {
            toggleUIAction.performed += OnToggleUI;
        }
        else
        {
            Debug.LogWarning($"[InputReader] ToggleUI action '{toggleUIActionName}' not found in map '{actionMapName}'");
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

        // ✅ NEW
        if (toggleUIAction != null)
        {
            toggleUIAction.performed -= OnToggleUI;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx) => MoveValue = ctx.ReadValue<Vector2>();

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        bool held = ctx.ReadValueAsButton();
        if (SprintHeld == held) return;

        SprintHeld = held;
        Debug.Log($"[InputReader] SprintHeld = {SprintHeld} (control={ctx.control?.path})");
    }

    private void OnJump(InputAction.CallbackContext ctx) => JumpPressed?.Invoke();
    private void OnJumpCanceled(InputAction.CallbackContext ctx) => JumpReleased?.Invoke();

    // ✅ NEW
    private void OnToggleUI(InputAction.CallbackContext ctx)
    {
        Debug.Log("[InputReader] ToggleUI pressed (Tab)");
        ToggleUI?.Invoke();
    }
}
