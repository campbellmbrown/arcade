using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class PixelateEffect(GraphicsDevice graphicsDevice, int pixelation) : IGameEffect
{
    RenderTarget2D _pixelationRenderTarget = new(
        graphicsDevice,
        graphicsDevice.PresentationParameters.BackBufferWidth / pixelation,
        graphicsDevice.PresentationParameters.BackBufferHeight / pixelation
    );

    public bool IsEnabled { get; set; } = true;

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
        if (!IsEnabled)
        {
            throw new InvalidOperationException("Effect is not enabled.");
        }

        // First draw to the pixelation render target at a lower resolution
        graphicsDevice.SetRenderTarget(_pixelationRenderTarget);
        graphicsDevice.Clear(Color.Transparent);
        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        renderer.SpriteBatch.Draw(source, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1.0f / pixelation, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();

        // Then draw the pixelated render target to the destination at full resolution
        graphicsDevice.SetRenderTarget(destination);
        graphicsDevice.Clear(Color.Transparent);
        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        renderer.SpriteBatch.Draw(_pixelationRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, pixelation, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        _pixelationRenderTarget = new RenderTarget2D(
            graphicsDevice,
            graphicsDevice.PresentationParameters.BackBufferWidth / pixelation,
            graphicsDevice.PresentationParameters.BackBufferHeight / pixelation
        );
    }
}
