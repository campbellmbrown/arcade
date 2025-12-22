using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.World;

public interface IEntity : IFrameTickable, IVisual
{
    Vector2 Direction { get; }
    Vector2 Position { get; }
    Vector2 Center { get; }
    float MovementSpeed { get; init; }
}

public class Entity : IEntity
{
    public Vector2 Direction { get; protected set; } = Vector2.Zero;
    public Vector2 Position { get; protected set; } = Vector2.Zero;
    public Vector2 Center => Position; // TODO: swap to collision center
    public float MovementSpeed { get; init; } = 0f;

    public virtual void FrameTick(IFrameTickService frameTickService)
    {
        if (Direction != Vector2.Zero)
        {
            Vector2 displacement = Vector2.Normalize(Direction) * MovementSpeed * frameTickService.TimeDiffSec;
            Position += displacement;
        }
        Direction = Vector2.Zero;
    }

    public virtual void Draw(IRenderer renderer)
    {
        // Do nothing by default
    }
}
