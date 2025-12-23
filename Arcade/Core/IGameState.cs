using Arcade.Visual;

namespace Arcade.Core;

public interface IGameState<TStateId> : IVisual, IFrameTickable where TStateId : struct, Enum
{
    void Enter(object? payload = null);

    StateSwitch<TStateId>? StateSwitch { get; set; }
}

public abstract class GameState<TStateId> : IGameState<TStateId> where TStateId : struct, Enum
{
    public StateSwitch<TStateId>? StateSwitch { get; set; }

    public virtual void Enter(object? payload = null)
    {
        // Do nothing by default
    }

    public abstract void FrameTick(IFrameTickService frameTickService);
    public abstract void Draw(IRenderer renderer);
}
