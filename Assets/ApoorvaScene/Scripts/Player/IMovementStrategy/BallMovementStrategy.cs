using UnityEngine;

public sealed class BallMovementStrategy : IMovementStrategy
{
    public void OnEnter(PlayerMovement ctx)
    {
        ctx.ResetHorizontalMomentum();
        ctx.SetRollVisualActive(true);
        ctx.SetBallBounceActive(true);
        ctx.ClearWallStickState();
    }

    public void OnExit(PlayerMovement ctx)
    {
        ctx.SetRollVisualActive(false);
        ctx.SetBallBounceActive(false);
    }

    public void Tick(PlayerMovement ctx, float dt, bool grounded)
    {
        ctx.MoveHorizontalBallMomentum(dt);

        // ✅ bounce logic only for ball, and only when active
        ctx.UpdateBallLandingBounce(dt, grounded);

        ctx.ApplyGravityOptimized(dt, grounded);

        ctx.ApplyBallRollVisual(dt);
    }
}
