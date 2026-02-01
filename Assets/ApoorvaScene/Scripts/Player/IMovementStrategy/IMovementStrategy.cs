public interface IMovementStrategy
{
    void OnEnter(PlayerMovement ctx);
    void OnExit(PlayerMovement ctx);
    void Tick(PlayerMovement ctx, float dt, bool grounded);
}
