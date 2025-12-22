using Arcade.Visual;

namespace Arcade.Core;

public interface IGameState : IVisual, IFrameTickable
{
    void Enter();
}
