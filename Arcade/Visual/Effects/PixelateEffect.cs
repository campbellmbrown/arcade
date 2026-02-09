using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class PixelateEffect(GraphicsDevice graphicsDevice) : IGameEffect
{
    const int Pixelation = 8;

    readonly RenderTarget2D _pixelationRenderTarget = new(
        graphicsDevice,
        graphicsDevice.PresentationParameters.BackBufferWidth / Pixelation,
        graphicsDevice.PresentationParameters.BackBufferHeight / Pixelation
    );

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
        // First draw to the pixelation render target at a lower resolution
        graphicsDevice.SetRenderTarget(_pixelationRenderTarget);
        graphicsDevice.Clear(Color.Transparent);
        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        renderer.SpriteBatch.Draw(source, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1.0f / Pixelation, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();

        // Then draw the pixelated render target to the destination at full resolution
        graphicsDevice.SetRenderTarget(destination);
        graphicsDevice.Clear(Color.Transparent);
        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        renderer.SpriteBatch.Draw(_pixelationRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Pixelation, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();
    }

    // TODO: handle the window resized event
}
