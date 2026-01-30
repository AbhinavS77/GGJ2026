using UnityEngine;

[DefaultExecutionOrder(-10)]
public sealed class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InputReader input;
    [SerializeField] private PlayerMovement movement;

    [Header("Optional (if you want controller to drive camera later)")]
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        if (movement != null)
            movement.SetController(this);

        if (movement != null && cameraTransform != null)
            movement.SetCameraTransform(cameraTransform);
    }

    private void Update()
    {
        if (input == null || movement == null) return;

        movement.SetMoveInput(input.MoveValue);
        //movement.SetSprintHeld(input.SprintHeld);
    }

    public Transform CameraTransform => cameraTransform;
}
