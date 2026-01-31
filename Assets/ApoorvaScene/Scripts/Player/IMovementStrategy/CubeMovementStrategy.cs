using UnityEngine;

public sealed class CubeMovementStrategy : IMovementStrategy
{
    public void OnEnter(PlayerMovement ctx)
    {
        ctx.SetRollVisualActive(false);
    }

    public void OnExit(PlayerMovement ctx)
    {
        ctx.ClearWallStickState();
    }

    public void Tick(PlayerMovement ctx, float dt, bool grounded)
    {
        ctx.MoveHorizontalImmediate(dt);

        // Wall stick only if airborne
        if (!grounded)
            ctx.HandleWallStick(dt);

        ctx.ApplyGravityOptimized(dt, grounded, allowStickOverride: true);
    }
}
