using UnityEngine;

public sealed class CubeMovementStrategy : IMovementStrategy
{
    public void OnEnter(PlayerMovement ctx)
    {
        ctx.SetRollVisualActive(false);
        ctx.SetBallBounceActive(false);
        ctx.ClearWallStickState();
    }

    public void OnExit(PlayerMovement ctx)
    {
        ctx.ClearWallStickState();
    }

    public void Tick(PlayerMovement ctx, float dt, bool grounded)
    {
        ctx.MoveHorizontalImmediate(dt);

        if (grounded)
            ctx.ClearWallStickState();
        else
            ctx.HandleWallStick(dt);   // ✅ dt REQUIRED

        ctx.ApplyGravityOptimized(dt, grounded, allowStickOverride: true);
    }
}
