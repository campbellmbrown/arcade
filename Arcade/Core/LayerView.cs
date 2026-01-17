using Arcade.Utility;
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

    /// <summary>
    /// Shifts the focus of the layer view by the given delta.
    /// </summary>
    /// <param name="delta">The delta to shift the focus by.</param>
    void ShiftFocus(Vector2 delta);

    void Rotate90();

    void RotateNeg90();

    void ZoomAtMouse(float zoomFactor);

    void WindowResized();
}

public class LayerView : ILayerView
{
    const int SCALE_FACTOR = 1;
    const float MINIMUM_ZOOM = 0.5f;
    const float MAXIMUM_ZOOM = 100f;

    readonly GraphicsDevice _graphicsDevice;
    readonly GameWindow _window;

    public LayerView(GraphicsDevice graphicsDevice, GameWindow window, float zoom)
    {
        _graphicsDevice = graphicsDevice;
        _window = window;
        Zoom = zoom;

        // Each layer has a different camera because they can have different positions/zooms.
        Camera = new OrthographicCamera(graphicsDevice)
        {
            MinimumZoom = MINIMUM_ZOOM,
            MaximumZoom = MAXIMUM_ZOOM,
        };

        Camera.ZoomIn(Zoom - SCALE_FACTOR);
    }

    public OrthographicCamera Camera { get; private set; }
    public Vector2 Origin => Camera.ScreenToWorld(Vector2.Zero);
    public Vector2 Size => new Vector2(_window.ClientBounds.Width, _window.ClientBounds.Height) / Camera.Zoom;
    public Vector2 Center => Camera.Center;
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

    public void Rotate90() => Camera.Rotate(MathF.PI / 2);

    public void RotateNeg90() => Camera.Rotate(-MathF.PI / 2);

    public void ZoomAtMouse(float zoomFactor)
    {
        var offsetFromCenter = MousePosition - Center;
        var afterZoomOffset = (zoomFactor - 1) * offsetFromCenter / 2;

        var prevZoom = Camera.Zoom;
        Camera.ZoomIn(Camera.Zoom * (zoomFactor - 1));

        if (Camera.Zoom != prevZoom)
        {
            Zoom = Camera.Zoom;
            Camera.LookAt(Center + afterZoomOffset);
        }
    }

    public void WindowResized()
    {
        var previousCenter = Camera.Center;
        Camera = new OrthographicCamera(_graphicsDevice)
        {
            MinimumZoom = MINIMUM_ZOOM,
            MaximumZoom = MAXIMUM_ZOOM,
        };
        Camera.ZoomIn(Zoom - SCALE_FACTOR);
        Camera.LookAt(previousCenter);
    }
}
