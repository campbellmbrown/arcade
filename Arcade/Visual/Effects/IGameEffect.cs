using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public interface IGameEffect
{
    void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination);
    void WindowResized();

    bool IsEnabled { get; set; }
}
