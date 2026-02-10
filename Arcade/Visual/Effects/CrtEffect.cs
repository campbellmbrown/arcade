using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public class CrtEffect : IGameEffect
{
    readonly GraphicsDevice _graphicsDevice;
    readonly Effect _effect;

    public CrtEffect(IContentService content, GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _effect = content.Effect.GetStandard(StandardEffectId.Crt);
        _effect.Parameters["hardScan"]?.SetValue(-1.0f);
        _effect.Parameters["hardPix"]?.SetValue(-3.0f);
        _effect.Parameters["warpX"]?.SetValue(0.031f);
        _effect.Parameters["warpY"]?.SetValue(0.041f);
        _effect.Parameters["maskDark"]?.SetValue(0.5f);
        _effect.Parameters["maskLight"]?.SetValue(1.5f);
        _effect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
        _effect.Parameters["shadowMask"]?.SetValue(3.0f);
        _effect.Parameters["brightboost"]?.SetValue(1.0f);
        _effect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
        _effect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
        _effect.Parameters["bloomAmount"]?.SetValue(0.15f);
        _effect.Parameters["shape"]?.SetValue(2.0f);

        Vector2 size = new(
            graphicsDevice.PresentationParameters.BackBufferWidth,
            graphicsDevice.PresentationParameters.BackBufferHeight
        );
        _effect.Parameters["textureSize"].SetValue(size);
        _effect.Parameters["videoSize"].SetValue(size);
        _effect.Parameters["outputSize"].SetValue(size);
    }

    public void ApplyEffect(IRenderer renderer, RenderTarget2D source, RenderTarget2D destination)
    {
        _graphicsDevice.SetRenderTarget(destination);
        _graphicsDevice.Clear(Color.Transparent);

        renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _effect);
        renderer.SpriteBatch.Draw(source, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        Vector2 size = new(
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );
        _effect.Parameters["textureSize"].SetValue(size);
        _effect.Parameters["videoSize"].SetValue(size);
        _effect.Parameters["outputSize"].SetValue(size);
    }
}
