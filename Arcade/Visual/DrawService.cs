using Arcade.Core;
using Arcade.Visual.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public interface IDrawService
{
    void RegisterEffect(IGameEffect effect);

    void Start(DrawType drawType);
    void Switch(DrawType drawType);
    void Finish();

    void WindowResized();

    DrawType DrawType { get; }

    ILayerView GuiLayer { get; }
    ILayerView WorldLayer { get; }

    IReadOnlyList<IGameEffect> Effects { get; }
}

public class DrawService : IDrawService
{
    readonly GraphicsDevice _graphicsDevice;
    readonly IRenderer _renderer;
    readonly List<IGameEffect> _effects = [];

    public DrawType DrawType { get; private set; }

    public ILayerView GuiLayer { get; private set; }
    public ILayerView WorldLayer { get; private set; }

    public IReadOnlyList<IGameEffect> Effects => _effects;

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

    RenderTarget2D _tmpRenderTarget;

    /// <summary>
    /// Render target for the world content without effects.
    /// </summary>
    RenderTarget2D _worldNoEffectsRenderTarget;

    public DrawService(IRenderContext renderContext, IRenderer renderer)
    {
        _graphicsDevice = renderContext.GraphicsDevice;
        _renderer = renderer;

        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _tmpRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

        GuiLayer = new LayerView(renderContext, zoom: 2);
        WorldLayer = new LayerView(renderContext, zoom: 4);
    }

    public void RegisterEffect(IGameEffect effect)
    {
        _effects.Add(effect);
    }

    public void Start(DrawType drawType)
    {
        DrawType = drawType;
        switch (drawType)
        {
            case DrawType.Gui:
                _graphicsDevice.SetRenderTarget(_guiRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = GuiLayer;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: GuiLayer.Camera.GetViewMatrix());
                break;
            case DrawType.World:
                _graphicsDevice.SetRenderTarget(_worldRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = WorldLayer;
                _renderer.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: WorldLayer.Camera.GetViewMatrix());
                break;
            case DrawType.WorldNoEffects:
                _graphicsDevice.SetRenderTarget(_worldNoEffectsRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.CurrentLayer = WorldLayer;
                _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: WorldLayer.Camera.GetViewMatrix());
                break;
        }
    }

    public void Switch(DrawType drawType)
    {
        _renderer.SpriteBatch.End();
        Start(drawType);
    }

    public void Finish()
    {
        _renderer.SpriteBatch.End();

        // (1) Apply effects to the world render target one by one
        RenderTarget2D source = _worldRenderTarget;
        RenderTarget2D destination = _tmpRenderTarget;
        RenderTarget2D final = _worldRenderTarget; // Assuming no effects
        foreach (var effect in _effects)
        {
            if (!effect.IsEnabled)
            {
                continue;
            }

            effect.ApplyEffect(_renderer, source, destination);
            final = destination;

            // Swap the render target references for the next effect
            (source, destination) = (destination, source);
        }

        // (2) Draw the world content to the back buffer.
        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.Black);
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderer.SpriteBatch.Draw(final, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();

        // (3) Draw the rest of the targets to the back buffer.
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderer.SpriteBatch.Draw(_worldNoEffectsRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.Draw(_guiRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _tmpRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

        foreach (var effect in _effects)
        {
            effect.WindowResized();
        }

        GuiLayer.WindowResized();
        WorldLayer.WindowResized();
    }
}
