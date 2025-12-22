using Arcade.Core.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Arcade.Core;

public interface ILayerView
{
    OrthographicCamera Camera { get; }

    /// <summary>
    /// Top left of the view.
    /// </summary>
    Vector2 Origin { get; }

    /// <summary>
    /// Width/Height of the view.
    /// </summary>
    Vector2 Size { get; }

    Vector2 Center { get; }

    Vector2 MousePosition { get; }

    float Zoom { get; }

    void Focus(Vector2 focusPoint);

    void ShiftFocus(Vector2 delta);

    void ZoomAtMouse(float zoomFactor);

    void WindowResized();
}

public class LayerView : ILayerView
{
    readonly GraphicsDevice _graphicsDevice;
    readonly GameWindow _window;

    const int SCALE_FACTOR = 1;

    public LayerView(GraphicsDevice graphicsDevice, GameWindow window, float zoom)
    {
        _graphicsDevice = graphicsDevice;
        _window = window;
        Zoom = zoom;

        // Each layer has a different camera because they can have different positions/zooms.
        Camera = new OrthographicCamera(graphicsDevice)
        {
            MinimumZoom = 0.5f,
            MaximumZoom = 100f,
        };

        Camera.ZoomIn(Zoom - SCALE_FACTOR);
    }

    public OrthographicCamera Camera { get; private set; }
    public Vector2 Origin => Camera.ScreenToWorld(Vector2.Zero);
    public Vector2 Size => new Vector2(_window.ClientBounds.Width, _window.ClientBounds.Height) / Camera.Zoom;
    public Vector2 Center => Origin + Size / 2;
    public Vector2 MousePosition
    {
        get
        {
            Vector2 mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
            return Camera.ScreenToWorld(mousePos.X, mousePos.Y);
        }
    }

    public float Zoom { get; private set; }

    public void Focus(Vector2 focusPoint) => Camera.LookAt(focusPoint);

    public void ShiftFocus(Vector2 delta) => Focus(Center + delta);

    public void ZoomAtMouse(float zoomFactor)
    {
        var offsetFromCenter = MousePosition - Center;
        var afterZoomOffset = (zoomFactor - 1) * offsetFromCenter / 2;

        Camera.ZoomIn(Camera.Zoom * (zoomFactor - 1));
        Zoom = Camera.Zoom;

        Camera.LookAt(Center + afterZoomOffset);
    }

    public void WindowResized()
    {
        Camera = new OrthographicCamera(_graphicsDevice);
        Camera.ZoomIn(Zoom - SCALE_FACTOR);
    }
}
