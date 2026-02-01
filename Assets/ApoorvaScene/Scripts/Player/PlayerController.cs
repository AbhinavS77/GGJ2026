using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-10)]
public sealed class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InputReader input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private MaskController masks;

    // ✅ NEW
    [Header("UI")]
    [SerializeField] private UIOverlayController uiOverlay;

    //[Header("Optional Camera Ref")]
    //[SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        //if (movement != null && cameraTransform != null)
        //    movement.SetCameraTransform(cameraTransform);
    }

    private void OnEnable()
    {
        if (input == null || movement == null) return;

        input.JumpPressed += OnJumpPressed;
        input.JumpReleased += OnJumpReleased;

        // ✅ NEW
        input.ToggleUI += OnToggleUI;

        Debug.Log("[PlayerController] Subscribed to input events");
    }

    private void OnDisable()
    {
        if (input == null || movement == null) return;

        input.JumpPressed -= OnJumpPressed;
        input.JumpReleased -= OnJumpReleased;

        // ✅ NEW
        input.ToggleUI -= OnToggleUI;

        Debug.Log("[PlayerController] Unsubscribed from input events");
    }

    private void Update()
    {
        if (input == null || movement == null) return;

        movement.SetMoveInput(input.MoveValue);
        movement.SetSprintHeld(input.SprintHeld);

        HandleMaskInput();
    }

    private void HandleMaskInput()
    {
        if (masks == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) masks.EquipIndex(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) masks.EquipIndex(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) masks.EquipIndex(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) masks.EquipIndex(3);
    }

    private void OnJumpPressed() => movement.JumpPressed();
    private void OnJumpReleased() => movement.JumpReleased();

    // ✅ NEW
    private void OnToggleUI()
    {
        if (uiOverlay == null)
        {
            Debug.LogWarning("[PlayerController] uiOverlay not assigned.");
            return;
        }

        uiOverlay.Toggle();
    }
}
