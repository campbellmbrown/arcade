using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class LightingEffect(IDrawService drawService, IContentService content, GraphicsDevice graphicsDevice) : IGameEffect
{
    readonly Effect _effect = content.Effect.GetStandard(StandardEffectId.Lighting);

    RenderTarget2D _lightRenderTarget = new(
        graphicsDevice,
        graphicsDevice.PresentationParameters.BackBufferWidth,
        graphicsDevice.PresentationParameters.BackBufferHeight
    );

    public bool IsEnabled { get; set; } = true;

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
        if (!IsEnabled)
        {
            throw new InvalidOperationException("Effect is not enabled.");
        }

        graphicsDevice.SetRenderTarget(_lightRenderTarget);
        graphicsDevice.Clear(Color.Black);

        if (renderer.HasLights)
        {
            renderer.SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Additive,
                SamplerState.PointClamp,
                transformMatrix: drawService.WorldLayer.Camera.GetViewMatrix()
            );

            while (renderer.HasLights)
            {
                var light = renderer.LightQueue.Dequeue();
                light.Draw(renderer);
            }
            renderer.SpriteBatch.End();
        }
        _effect.Parameters["lightMask"].SetValue(_lightRenderTarget);

        graphicsDevice.SetRenderTarget(destination);
        graphicsDevice.Clear(Color.Transparent);

        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _effect);
        renderer.SpriteBatch.Draw(source, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        _lightRenderTarget = new RenderTarget2D(
            graphicsDevice,
            graphicsDevice.PresentationParameters.BackBufferWidth,
            graphicsDevice.PresentationParameters.BackBufferHeight
        );
    }
}
