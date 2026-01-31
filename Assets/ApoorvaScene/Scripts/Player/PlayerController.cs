using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-10)]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private MaskController masks;

    private void OnEnable()
    {
        if (input == null || movement == null) return;
        input.JumpPressed += movement.JumpPressed;
        input.JumpReleased += movement.JumpReleased;
    }

    private void OnDisable()
    {
        if (input == null || movement == null) return;
        input.JumpPressed -= movement.JumpPressed;
        input.JumpReleased -= movement.JumpReleased;
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

        // ✅ Alpha1 / Digit1 switches to BOX (index 1)
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            masks.EquipIndex(1);

        // Optional: if you want a key to go back to Ball (index 0)
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            masks.EquipIndex(0);
    }
}
