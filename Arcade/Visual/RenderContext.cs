using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

/// <summary>
/// Shared rendering environment.
/// </summary>
public interface IRenderContext
{
    GraphicsDevice GraphicsDevice { get; }
    GameWindow Window { get; }
}

public sealed class RenderContext(GraphicsDevice graphicsDevice, GameWindow window) : IRenderContext
{
    public GraphicsDevice GraphicsDevice { get; } = graphicsDevice;
    public GameWindow Window { get; } = window;
}
