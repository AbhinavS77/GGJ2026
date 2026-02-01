using UnityEngine;

public sealed class DefaultMovementStrategy : IMovementStrategy
{
    public void OnEnter(PlayerMovement ctx)
    {
        ctx.SetRollVisualActive(false);
        ctx.SetBallBounceActive(false);
        ctx.ClearWallStickState();
    }

    public void OnExit(PlayerMovement ctx) { }

    public void Tick(PlayerMovement ctx, float dt, bool grounded)
    {
        ctx.MoveHorizontalImmediate(dt);
        ctx.ApplyGravityOptimized(dt, grounded);
    }
}
