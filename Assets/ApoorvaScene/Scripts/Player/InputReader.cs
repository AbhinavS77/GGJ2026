using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputReader : MonoBehaviour
{
    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset actions;

    [Header("Action Map / Actions")]
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private string sprintActionName = "Sprint";
    [SerializeField] private string jumpActionName = "Jump";

    public Vector2 MoveValue { get; private set; }
    public bool SprintHeld { get; private set; }

    public event Action Interact;
    public event Action JumpPressed;
    public event Action<bool> SprintChanged;

    private InputActionMap map;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private void Awake()
    {
        if (actions == null)
        {
            enabled = false;
            return;
        }

        map = actions.FindActionMap(actionMapName, true);

        moveAction = map.FindAction(moveActionName, true);
        interactAction = map.FindAction(interactActionName, false);
        sprintAction = map.FindAction(sprintActionName, false);
        jumpAction = map.FindAction(jumpActionName, false);

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        if (interactAction != null)
            interactAction.performed += OnInteract;

        if (jumpAction != null)
            jumpAction.performed += OnJump;

        if (sprintAction != null)
        {
            sprintAction.performed += OnSprint;
            sprintAction.canceled += OnSprint;
        }
    }

    private void OnEnable()
    {
        if (map != null) map.Enable();
    }

    private void OnDisable()
    {
        if (map != null) map.Disable();
    }

    private void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
        }

        if (interactAction != null)
            interactAction.performed -= OnInteract;

        if (jumpAction != null)
            jumpAction.performed -= OnJump;

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprint;
            sprintAction.canceled -= OnSprint;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveValue = ctx.ReadValue<Vector2>();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        Interact?.Invoke();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        JumpPressed?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        bool held = ctx.ReadValueAsButton();
        if (SprintHeld == held) return;

        SprintHeld = held;
        SprintChanged?.Invoke(SprintHeld);
    }
}
