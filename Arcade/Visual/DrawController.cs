using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public enum DrawType
{
    Gui,
    World,
    WorldNoEffects,
#if LIGHT_EFFECT
    Light,
#endif
}

public interface IDrawController
{
    void Start(DrawType drawType);
    void Switch(DrawType drawType);
    void Finish();

    void WindowResized();

    DrawType DrawType { get; }

    ILayerView GuiLayerView { get; }
    ILayerView WorldLayerView { get; }
}

public class DrawController : IDrawController
{
    readonly GraphicsDevice _graphicsDevice;
    readonly IRenderer _renderer;

    public DrawType DrawType { get; private set; }

    public ILayerView GuiLayerView { get; private set; }
    public ILayerView WorldLayerView { get; private set; }

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
    /// Render target for the world content.
    /// </summary>
    RenderTarget2D _worldRenderTarget;

    /// <summary>
    /// Render target for the world content without effects.
    /// </summary>
    RenderTarget2D _worldNoEffectsRenderTarget;

#if LIGHT_EFFECT
    /// <summary>
    /// A special render target for drawing lights for the world content target.
    /// This render target will be an input to the light effect.
    /// </summary>
    RenderTarget2D _lightRenderTarget;
#endif

#if LIGHT_EFFECT
    // Effects
    readonly Effect _lightingEffect;
#endif

    public DrawController(ContentManager content, GraphicsDevice graphicsDevice, GameWindow window, IRenderer renderer)
    {
        _graphicsDevice = graphicsDevice;
        _renderer = renderer;

        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
#if LIGHT_EFFECT
        _lightRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
#endif

        GuiLayerView = new LayerView(_graphicsDevice, window, zoom: 2);
        WorldLayerView = new LayerView(_graphicsDevice, window, zoom: 4);

#if LIGHT_EFFECT
        _lightingEffect = content.Load<Effect>("effects/lighting");
#endif
    }

    public void Start(DrawType drawType)
    {
        DrawType = drawType;
        switch (drawType)
        {
            case DrawType.Gui:
                _graphicsDevice.SetRenderTarget(_guiRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = GuiLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: GuiLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.World:
                _graphicsDevice.SetRenderTarget(_worldRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = WorldLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: WorldLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.WorldNoEffects:
                _graphicsDevice.SetRenderTarget(_worldNoEffectsRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = WorldLayerView;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: WorldLayerView.Camera.GetViewMatrix());
                break;
#if LIGHT_EFFECT
            case DrawType.Light:
                throw new InvalidOperationException("The light target cannot be switched to.");
#endif
        }
    }

    public void Switch(DrawType drawType)
    {
        _renderer.SpriteBatch.End();
        Start(drawType);
    }

    public void Finish()
    {
#if LIGHT_EFFECT
        // (1) Draw lights to the light target.
        if (_renderer.HasLights)
        {
            _renderer.SpriteBatch.End();
            _graphicsDevice.SetRenderTarget(_lightRenderTarget);
            _graphicsDevice.Clear(Color.Black);
            _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: WorldLayerView.Camera.GetViewMatrix());

            while (_renderer.HasLights)
            {
                var light = _renderer.LightQueue.Dequeue();
                _renderer.SpriteBatch.Draw(light.Texture, light.Position, null, light.Color * light.Opacity, 0f, light.RotationOrigin, light.Scale, SpriteEffects.None, 0f);
            }
        }
#endif
        _renderer.SpriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.Black);

#if LIGHT_EFFECT
        // (2) Draw the world content to the back buffer with the point light as a mask.
        _lightingEffect.Parameters["lightMask"].SetValue(_lightRenderTarget);
        var effect = _lightingEffect;
#else
        Effect? effect = null;
#endif
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: effect);
        _renderer.SpriteBatch.Draw(_worldRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
#if LIGHT_EFFECT
        _renderer.SpriteBatch.End();

        // (3) Draw the rest of the targets to the back buffer.
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
#endif
        _renderer.SpriteBatch.Draw(_worldNoEffectsRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.Draw(_guiRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
#if LIGHT_EFFECT
        _lightRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
#endif

        GuiLayerView.WindowResized();
        WorldLayerView.WindowResized();
    }
}
