using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class LightingEffect(IDrawService drawService, GraphicsDevice graphicsDevice) : IGameEffect
{
    const string EffectPath = "Content/effects/lighting.mgfxo";

    RenderTarget2D _lightRenderTarget = new(
        graphicsDevice,
        graphicsDevice.PresentationParameters.BackBufferWidth,
        graphicsDevice.PresentationParameters.BackBufferHeight
    );

    public Effect Effect { get; } = new(graphicsDevice, File.ReadAllBytes(EffectPath));

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
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
                renderer.SpriteBatch.Draw(
                    light.Texture,
                    light.Position,
                    null,
                    light.Color * light.Opacity,
                    0f,
                    light.RotationOrigin,
                    light.Scale,
                    SpriteEffects.None,
                    0f
                );
            }
            renderer.SpriteBatch.End();
        }
        Effect.Parameters["lightMask"].SetValue(_lightRenderTarget);

        graphicsDevice.SetRenderTarget(destination);
        graphicsDevice.Clear(Color.Transparent);

        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: Effect);
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
