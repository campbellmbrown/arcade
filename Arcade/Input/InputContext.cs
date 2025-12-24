using Arcade.Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Input;

public interface IScrollable
{
    RectangleF ScrollArea { get; }
    void OnScroll(int delta);
}

/// <summary>
/// Context for inputs that requires layer-specific handling. This context is bound to a specific layer view.
/// </summary>
public interface IInputContext
{
    /// <summary>
    /// Registers pan handlers for this input context.
    /// </summary>
    /// <remarks>
    /// Panning works by recording the mouse position in the context's layer view when panning starts. If the mouse
    /// moves away from that position while panning is active, the delta is reported through the callbacks. The delta is
    /// the current mouse position minus the recorded position. The caller is responsible for updating the view based on
    /// the delta. For example, if the view is being panned by a delta of (5, 10), the caller should move the camera by
    /// (-5, -10) to realign the mouse position with the original pan start position, creating the effect of panning.
    /// </remarks>
    /// <param name="onPanStart">The action to invoke when panning starts.</param>
    /// <param name="onPan">The action to invoke when panning.</param>
    /// <param name="onPanEnd">The action to invoke when panning ends.</param>
    void RegisterPan(Action<Vector2>? onPanStart, Action<Vector2>? onPan, Action<Vector2>? onPanEnd);

    /// <summary>
    /// Registers a scrollable to this input context.
    /// </summary>
    /// <param name="scrollable">The scrollable to register.</param>
    void RegisterScrollable(IScrollable scrollable);

    void HandlePanStart();
    void HandlePan();
    void HandlePanEnd();

    /// <summary>
    /// Handles a scroll event. Consumes the event if handled so it can't be propagated further.
    /// </summary>
    /// <param name="delta">The scroll delta.</param>
    /// <returns>True if the scroll event was handled, false otherwise.</returns>
    bool HandleScroll(int delta);
}

public class InputContext(ILayerView layerView) : IInputContext
{
    Action<Vector2>? _onPanStart;
    Action<Vector2>? _onPan;
    Action<Vector2>? _onPanEnd;
    bool _panRegistered = false;

    readonly List<IScrollable> _scrollables = [];

    Vector2 _panPosition = Vector2.Zero;

    public void RegisterPan(Action<Vector2>? onPanStart, Action<Vector2>? onPan, Action<Vector2>? onPanEnd)
    {
        if ((onPanStart == null) && (onPan == null) && (onPanEnd == null))
        {
            throw new ArgumentException("All pan callbacks cannot be null.");
        }
        if ((_onPanStart != null) || (_onPan != null) || (_onPanEnd != null))
        {
            throw new InvalidOperationException("Pan callbacks have already been registered.");
        }
        _onPanStart = onPanStart;
        _onPan = onPan;
        _onPanEnd = onPanEnd;
        _panRegistered = true;
    }

    public void RegisterScrollable(IScrollable scrollable)
    {
        _scrollables.Add(scrollable);
    }


    public void HandlePanStart()
    {
        if (!_panRegistered)
        {
            return;
        }
        _panPosition = layerView.MousePosition;
        _onPanStart?.Invoke(_panPosition);
    }

    public void HandlePan()
    {
        if (!_panRegistered)
        {
            return;
        }
        var delta = layerView.MousePosition - _panPosition;
        _onPan?.Invoke(delta);
    }

    public void HandlePanEnd()
    {
        if (!_panRegistered)
        {
            return;
        }
        var delta = layerView.MousePosition - _panPosition;
        _onPanEnd?.Invoke(delta);
    }

    public bool HandleScroll(int delta)
    {
        foreach (var scrollable in _scrollables)
        {
            if (scrollable.ScrollArea.Contains(layerView.MousePosition))
            {
                scrollable.OnScroll(delta);
                return true;
            }
        }
        return false;
    }
}
