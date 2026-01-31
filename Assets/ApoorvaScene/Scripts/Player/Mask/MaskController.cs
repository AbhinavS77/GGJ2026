using System;
using UnityEngine;

public sealed class MaskController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MaskLibrary library;
    [SerializeField] private int startMaskIndex = 0;

    [Header("Refs")]
    [SerializeField] private MaskVisual visual;
    [SerializeField] private MaskColliderApplier colliderApplier;
    [SerializeField] private PlayerMovement movement;

    [Tooltip("Optional fallback mesh if mask.visualPrefab is null")]
    [SerializeField] private GameObject capsule;

    [Header("Unlocks (Jam Simple)")]
    [SerializeField] private int unlockedCount = 2;

    [Header("Debug")]
    [SerializeField] private bool equipOnStart = true;

    // ✅ NEW
    [Header("Ball Spawn Rotation")]
    [SerializeField] private Vector3 ballSpawnEuler = new Vector3(0f, 180f, 0f);

    public MaskDefinition Current { get; private set; }
    public int CurrentIndex { get; private set; }

    public event Action<MaskDefinition> OnMaskChanged;

    private void Awake()
    {
        if (visual == null) visual = GetComponentInChildren<MaskVisual>(true);
        if (colliderApplier == null) colliderApplier = GetComponent<MaskColliderApplier>();
        if (movement == null) movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (equipOnStart)
            EquipIndex(startMaskIndex);
    }

    public bool EquipIndex(int index)
    {
        if (library == null || library.masks == null || library.masks.Length == 0) return false;

        index = Mathf.Clamp(index, 0, library.masks.Length - 1);
        if (index >= unlockedCount) return false;

        var mask = library.GetByIndex(index);
        if (mask == null) return false;

        Current = mask;
        CurrentIndex = index;

        // capsule fallback
        if (capsule != null)
            capsule.SetActive(mask.visualPrefab == null);

        // spawn visual + collider
        visual?.Apply(mask);
        colliderApplier?.Apply(mask);

        // ✅ NEW: force ball facing direction by applying local rotation after spawn
        if (mask.enableBounce) // ball mask
        {
            Transform t = visual != null ? visual.CurrentVisualTransform : null;
            if (t != null)
                t.localRotation = Quaternion.Euler(ballSpawnEuler);
        }

        if (movement != null)
        {
            movement.SetSpeedMultiplier(mask.speedMultiplier);
            movement.SetGravityMultiplier(mask.gravityMultiplier);
            movement.SetJumpProfile(mask.jump);

            // strategy selection
            if (mask.enableBounce)
                movement.SetStrategy(new BallMovementStrategy());
            else if (mask.enableWallStick)
                movement.SetStrategy(new CubeMovementStrategy());
            else
                movement.SetStrategy(new DefaultMovementStrategy());

            // roll target
            Transform rollTarget =
                (visual != null && visual.CurrentVisualTransform != null)
                    ? visual.CurrentVisualTransform
                    : (capsule != null ? capsule.transform : null);

            movement.SetBallVisual(rollTarget);
        }

        Debug.Log($"Equipped Mask: {mask.displayName} ({mask.id})");
        OnMaskChanged?.Invoke(mask);
        return true;
    }
}
