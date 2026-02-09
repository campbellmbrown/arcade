using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public interface IGameEffect
{
    Effect Effect { get; }

    void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination);
}
