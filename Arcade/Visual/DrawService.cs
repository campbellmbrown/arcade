using Arcade.Core;
using Arcade.Visual.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public interface IDrawService
{
    void RegisterEffect(IGameEffect effect);

    /// <summary>
    /// Crop the width of the world back buffers. The cropping will be centered on the screen.
    /// If this is not called, the entire backbuffer width will be drawn to.
    /// </summary>
    /// <param name="width">The width to crop the world back buffers to in screen units, must be greater than 0</param>
    void CropWorldWidth(int width);

    /// <summary>
    /// Crop the height of the world back buffers. The cropping will be centered on the screen.
    /// If this is not called, the entire backbuffer height will be drawn to.
    /// </summary>
    /// <param name="height">The height to crop the world back buffers to in screen units, must be greater than 0</param>
    void CropWorldHeight(int height);

    void Start(DrawType drawType);
    void Switch(DrawType drawType);
    void Finish();

    void WindowResized();

    void TogglEffect<T>() where T : IGameEffect;

    DrawType DrawType { get; }

    ILayerView GuiLayer { get; }
    ILayerView WorldLayer { get; }
}

public class DrawService : IDrawService
{
    readonly GraphicsDevice _graphicsDevice;
    readonly IRenderer _renderer;
    readonly List<IGameEffect> _effects = [];

    int? _worldCropWidth = null;
    int? _worldCropHeight = null;
    Rectangle? _worldCropRectangle = null;

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

    public DrawService(IRenderContext renderContext, IRenderer renderer, int guiZoom = 2, int worldZoom = 4)
    {
        _graphicsDevice = renderContext.GraphicsDevice;
        _renderer = renderer;

        var width = _graphicsDevice.PresentationParameters.BackBufferWidth;
        var height = _graphicsDevice.PresentationParameters.BackBufferHeight;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _tmpRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);

        GuiLayer = new LayerView(renderContext, guiZoom);
        WorldLayer = new LayerView(renderContext, worldZoom);
        UpdateWorldCropRectangle();
    }

    public DrawType DrawType { get; private set; }

    public ILayerView GuiLayer { get; private set; }
    public ILayerView WorldLayer { get; private set; }

    public void RegisterEffect(IGameEffect effect)
    {
        _effects.Add(effect);
    }

    public void CropWorldWidth(int width)
    {
        _worldCropWidth = width < 1 ? null : width;
        UpdateWorldCropRectangle();
    }

    public void CropWorldHeight(int height)
    {
        _worldCropHeight = height < 1 ? null : height;
        UpdateWorldCropRectangle();
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

        Vector2 worldPosition = _worldCropRectangle is null
            ? Vector2.Zero
            : new Vector2(_worldCropRectangle.Value.X, _worldCropRectangle.Value.Y);

        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderer.SpriteBatch.Draw(final, worldPosition, _worldCropRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();

        // (3) Draw the rest of the targets to the back buffer.
        _renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderer.SpriteBatch.Draw(_worldNoEffectsRenderTarget, worldPosition, _worldCropRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        // GUI is intentionally always drawn full-screen without cropping.
        _renderer.SpriteBatch.Draw(_guiRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _renderer.SpriteBatch.End();
    }

    public void WindowResized()
    {
        var width = _graphicsDevice.PresentationParameters.BackBufferWidth;
        var height = _graphicsDevice.PresentationParameters.BackBufferHeight;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _worldRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _tmpRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        _worldNoEffectsRenderTarget = new RenderTarget2D(_graphicsDevice, width, height);

        foreach (var effect in _effects)
        {
            effect.WindowResized();
        }

        GuiLayer.WindowResized();
        WorldLayer.WindowResized();
        UpdateWorldCropRectangle();
    }

    public void TogglEffect<T>() where T : IGameEffect
    {
        foreach (var effect in _effects)
        {
            if (effect is T)
            {
                effect.IsEnabled = !effect.IsEnabled;
            }
        }
    }

    void UpdateWorldCropRectangle()
    {
        var backBufferWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
        var backBufferHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

        var width = _worldCropWidth is null ? backBufferWidth : Math.Clamp(_worldCropWidth.Value, 1, backBufferWidth);
        var height = _worldCropHeight is null ? backBufferHeight : Math.Clamp(_worldCropHeight.Value, 1, backBufferHeight);

        if ((width == backBufferWidth) && (height == backBufferHeight))
        {
            _worldCropRectangle = null;
            return;
        }

        var x = (backBufferWidth - width) / 2;
        var y = (backBufferHeight - height) / 2;
        _worldCropRectangle = new Rectangle(x, y, width, height);
    }
}
