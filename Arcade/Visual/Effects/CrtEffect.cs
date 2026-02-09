using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class CrtEffect : IGameEffect
{
    // TODO: move to content service
    const string EffectPath = "Content/effects/crt.mgfxo";

    readonly GraphicsDevice _graphicsDevice;

    public Effect Effect { get; }

    public CrtEffect(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        Effect = new Effect(graphicsDevice, File.ReadAllBytes(EffectPath));
        Effect.Parameters["hardScan"]?.SetValue(-1.0f);
        Effect.Parameters["hardPix"]?.SetValue(-3.0f);
        Effect.Parameters["warpX"]?.SetValue(0.031f);
        Effect.Parameters["warpY"]?.SetValue(0.041f);
        Effect.Parameters["maskDark"]?.SetValue(0.5f);
        Effect.Parameters["maskLight"]?.SetValue(1.5f);
        Effect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
        Effect.Parameters["shadowMask"]?.SetValue(3.0f);
        Effect.Parameters["brightboost"]?.SetValue(1.0f);
        Effect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
        Effect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
        Effect.Parameters["bloomAmount"]?.SetValue(0.15f);
        Effect.Parameters["shape"]?.SetValue(2.0f);

        Vector2 size = new(
            graphicsDevice.PresentationParameters.BackBufferWidth,
            graphicsDevice.PresentationParameters.BackBufferHeight
        );
        Effect.Parameters["textureSize"].SetValue(size);
        Effect.Parameters["videoSize"].SetValue(size);
        Effect.Parameters["outputSize"].SetValue(size);
    }

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
        _graphicsDevice.SetRenderTarget(destination);
        _graphicsDevice.Clear(Color.Transparent);

        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: Effect);
        renderer.SpriteBatch.Draw(source, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();
    }
}
