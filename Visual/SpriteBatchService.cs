using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public enum DrawType
{
    Gui,
    Main,
    MainNoEffects,
    Light,
}

public interface ISpriteBatchService
{
    void Start(DrawType drawType);
    void Switch(DrawType drawType);
    void Finish();

    void WindowResized();

    ILayerView GuiLayerView { get; }
    ILayerView MainLayerView { get; }
}

public class SpriteBatchService : ISpriteBatchService
{
    readonly GraphicsDevice _graphicsDevice;
    readonly IRenderer _renderer;

    public ILayerView GuiLayerView { get; private set; }
    public ILayerView MainLayerView { get; private set; }

    /* Render targets
    *
    * Instead of drawing our sprites to the back buffer we can instruct the GraphicsDevice to draw
    * to a render target instead. A render target is essentially an image that we are drawing to,
    * and then when we are done drawing to that render target we can draw it to the back buffer like
    * a regular texture. This is useful for:
    *
    * - Applying effects to a specific layer
    * - Applying global effects to all the layers (or specific layers) when we draw to the back buffer
    * - Having different cameras for each layer (e.g. a menu overlay vs game content)
    */

    /// <summary>
    /// Render target for content that doesn't move with the player/game camera.
    /// </summary>
    RenderTarget2D _guiRenderTarget;

    /// <summary>
    /// Render target for the main content.
    /// </summary>
    RenderTarget2D _mainRenderTarget;

    /// <summary>
    /// Render target for the main content without effects.
    /// </summary>
    RenderTarget2D _mainNoEffectsRenderTarget;

    /// <summary>
    /// A special render target for drawing lights for the main content target.
    /// This render target will be an input to the light effect.
    /// </summary>
    RenderTarget2D _lightRenderTarget;

    // Effects
    readonly Effect _lightingEffect;

    public SpriteBatchService(ContentManager content, GraphicsDevice graphicsDevice, GameWindow window, IRenderer renderer)
    {
        _graphicsDevice = graphicsDevice;
        _renderer = renderer;

        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _mainRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _mainNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _lightRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

        GuiLayerView = new LayerView(_graphicsDevice, window, zoom: 2);
        MainLayerView = new LayerView(_graphicsDevice, window, zoom: 4);

        _lightingEffect = content.Load<Effect>("effects/lighting");
    }

    public void Start(DrawType drawType)
    {
        switch (drawType)
        {
            case DrawType.Gui:
                _graphicsDevice.SetRenderTarget(_guiRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = GuiLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: GuiLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.Main:
                _graphicsDevice.SetRenderTarget(_mainRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = MainLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.MainNoEffects:
                _graphicsDevice.SetRenderTarget(_mainNoEffectsRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = MainLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.Light:
                throw new InvalidOperationException("The light target cannot be switched to.");
        }
    }

    public void Switch(DrawType drawType)
    {
        _renderer.SpriteBatch.End();
        Start(drawType);
    }

    public void Finish()
    {
        // (1) Draw lights to the light target.
        if (_renderer.HasLights)
        {
            _renderer.SpriteBatch.End();
            _graphicsDevice.SetRenderTarget(_lightRenderTarget);
            _graphicsDevice.Clear(Color.Black);
            _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());

            while (_renderer.HasLights)
            {
                var light = _renderer.LightQueue.Dequeue();
                _renderer.SpriteBatch.Draw(light.Texture, light.Position, null, light.Color * light.Opacity, 0f, light.RotationOrigin, light.Scale, SpriteEffects.None, 0f);
            }
        }
        _renderer.SpriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.Black);

        // (2) Draw the main content to the back buffer with the point light as a mask.
        _lightingEffect.Parameters["lightMask"].SetValue(_lightRenderTarget);
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: _lightingEffect);
        _renderer.SpriteBatch.Draw(_mainRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();

        // (3) Draw the rest of the targets to the back buffer.
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderer.SpriteBatch.Draw(_mainNoEffectsRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.Draw(_guiRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _mainRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _mainNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _lightRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

        GuiLayerView.WindowResized();
        MainLayerView.WindowResized();
    }
}
