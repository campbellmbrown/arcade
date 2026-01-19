using Arcade.Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Input;

public interface IClickable : IHoverable
{
    bool IsLatched { get; set; }

    void OnLatch();
    void OnRelease();
}

public interface IClickDraggable : IClickable
{
    void OnDrag(Vector2 position);
}

public interface IScrollable : IHoverable
{
    void OnScroll(int delta);
}

public interface IHoverable
{
    RectangleF InteractionArea { get; }
    bool IsHovering { get; set; }
}

// TODO: update all of this documentation

/// <summary>
/// Context for inputs that requires layer-specific handling. This context is bound to a specific layer view.
/// </summary>
public interface IInputContext
{
    /// <summary>
    /// Registers a clickable to this input context.
    /// </summary>
    /// <param name="clickable">The clickable to register.</param>
    void RegisterLeftClickSingleShot(IClickable clickable);

    /// <summary>
    /// Registers a click-draggable to this input context.
    /// </summary>
    /// <param name="clickDraggable">The click-draggable to register.</param>
    void RegisterLeftClickDraggable(IClickDraggable clickDraggable);

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

    /// <summary>
    /// Registers a hoverable to this input context.
    /// </summary>
    /// <param name="hoverable">The hoverable to register.</param>
    void RegisterHoverable(IHoverable hoverable);

    void TearDown();

    void HandlePanStart();
    void HandlePan();
    void HandlePanEnd();

    void HandleLeftClickStart(InputEvent inputEvent);
    void HandleLeftClickHold(InputEvent inputEvent);
    void HandleLeftClickRelease(InputEvent inputEvent);

    /// <summary>
    /// Handles a scroll event. Consumes the event if handled so it can't be propagated further.
    /// </summary>
    /// <param name="delta">The scroll delta.</param>
    /// <returns>True if the scroll event was handled, false otherwise.</returns>
    void HandleScroll(int delta, InputEvent inputEvent);

    /// <summary>
    /// Handles hover events. Consumes the event if handled so it can't be propagated further.
    /// </summary>
    /// <returns>True if a hover event was handled, false otherwise.</returns>
    void HandleHover(InputEvent inputEvent);

    /// <summary>
    /// The default left click action for this input context.
    /// </summary>
    /// <remarks>
    /// This action is invoked when a left click occurs in the layer view and no other UI element consumes the click.
    /// Note that if several input contexts have a default left click action registered, only the topmost context's
    /// action will be invoked.
    /// </remarks>
    Action<Vector2>? DefaultLeftClick { get; set; }
}

public class InputContext(ILayerView layerView) : IInputContext
{
    Action<Vector2>? _onPanStart;
    Action<Vector2>? _onPan;
    Action<Vector2>? _onPanEnd;
    bool _panRegistered = false;

    readonly List<IClickable> _leftClickSingleShots = [];
    readonly List<IClickDraggable> _leftClickDraggables = [];
    readonly List<IScrollable> _scrollables = [];
    readonly List<IHoverable> _hoverables = [];

    IHoverable? _hovered = null;
    IClickable? _latchedClickable = null;
    IClickDraggable? _latchedClickDraggable = null;

    Vector2 _panPosition = Vector2.Zero;

    public Action<Vector2>? DefaultLeftClick { get; set; }

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

    public void RegisterLeftClickSingleShot(IClickable clickable)
    {
        _leftClickSingleShots.Add(clickable);
    }

    public void RegisterLeftClickDraggable(IClickDraggable clickDraggable)
    {
        _leftClickDraggables.Add(clickDraggable);
    }

    public void RegisterScrollable(IScrollable scrollable)
    {
        _scrollables.Add(scrollable);
    }

    public void RegisterHoverable(IHoverable hoverable)
    {
        _hoverables.Add(hoverable);
    }

    public void TearDown()
    {
        _onPanStart = null;
        _onPan = null;
        _onPanEnd = null;
        _panRegistered = false;
        _leftClickSingleShots.Clear();
        _leftClickDraggables.Clear();
        _scrollables.Clear();
        _hoverables.Clear();
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

    public void HandleLeftClickStart(InputEvent inputEvent)
    {
        if (inputEvent.LeftClickConsumed)
        {
            return;
        }

        if (_hovered is IClickDraggable clickDraggable)
        {
            _latchedClickDraggable = clickDraggable;
            _latchedClickDraggable.IsLatched = true;
            _latchedClickDraggable.OnLatch();
            inputEvent.LeftClickConsumed = true;
        }
        else if (_hovered is IClickable clickable)
        {
            _latchedClickable = clickable;
            _latchedClickable.IsLatched = true;
            _latchedClickable.OnLatch();
            inputEvent.LeftClickConsumed = true;
        }
        else if (DefaultLeftClick != null)
        {
            // No clickable or draggable consumed the click, invoke the default left click action.
            // Note: this means that if the top layer context has a default left click action, it will
            // always consume the event, and no lower layer context will ever receive it.
            DefaultLeftClick(layerView.MousePosition);
            inputEvent.LeftClickConsumed = true;
        }
    }

    public void HandleLeftClickHold(InputEvent inputEvent)
    {
        if (inputEvent.LeftClickConsumed)
        {
            return;
        }

        if (_latchedClickDraggable != null)
        {
            // No need to check if still hovering, latched click-draggables receive all hold events until released
            _latchedClickDraggable.OnDrag(layerView.MousePosition);
            inputEvent.LeftClickConsumed = true;
        }
    }

    public void HandleLeftClickRelease(InputEvent inputEvent)
    {
        if (inputEvent.LeftClickConsumed)
        {
            return;
        }

        // no need to check the consumed flag, we can only release a clickable or
        if (_latchedClickable != null)
        {
            // Make sure that we are still over the clickable we pressed
            // This allows the user to cancel a click by moving the mouse away before releasing
            if (_latchedClickable.IsHovering)
            {
                _latchedClickable.OnRelease();
            }
            _latchedClickable.IsLatched = false;
            _latchedClickable = null;
            inputEvent.LeftClickConsumed = true;
        }
        else if (_latchedClickDraggable != null)
        {
            _latchedClickDraggable.IsLatched = false;
            _latchedClickDraggable.OnRelease();
            _latchedClickDraggable = null;
            inputEvent.LeftClickConsumed = true;
        }
    }

    public void HandleScroll(int delta, InputEvent inputEvent)
    {
        if (inputEvent.ScrollConsumed)
        {
            return;
        }

        if (_hovered is IScrollable scrollable)
        {
            scrollable.OnScroll(delta);
            inputEvent.ScrollConsumed = true;
        }
    }

    public void HandleHover(InputEvent inputEvent)
    {
        // TODO: bug - if I hover over a hoverable, then move to a clickable, the hoverable stays hovered, and the
        // clickable becomes hovered. Both are hovered at the same time. Need to fix that.

        if (inputEvent.HoverConsumed)
        {
            // Another context has already consumed the hover event, nothing on this context can be hovered
            return;
        }

        if (_latchedClickable != null || _latchedClickDraggable != null)
        {
            // Nothing else can be hovered while a clickable or click-draggable is latched
            inputEvent.HoverConsumed = true;
        }

        _hovered = null;

        var mousePosition = layerView.MousePosition;
        CheckClickableHover(mousePosition, inputEvent);
        CheckClickDraggableHover(mousePosition, inputEvent);
        CheckScrollableHover(mousePosition, inputEvent);
        CheckRemainingHoverables(mousePosition, inputEvent);
    }

    void CheckClickableHover(Vector2 mousePosition, InputEvent inputEvent)
    {
        foreach (var clickable in _leftClickSingleShots)
        {
            if (_latchedClickable != null)
            {
                // Only the latched clickable can be hovered
                if ((clickable == _latchedClickable) && clickable.InteractionArea.Contains(mousePosition))
                {
                    ConsumeHover(clickable, inputEvent);
                }
                else
                {
                    clickable.IsHovering = false;
                }
            }
            else if (!inputEvent.HoverConsumed && clickable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(clickable, inputEvent);
            }
            else
            {
                clickable.IsHovering = false;
            }
        }
    }

    void CheckClickDraggableHover(Vector2 mousePosition, InputEvent inputEvent)
    {
        foreach (var clickDraggable in _leftClickDraggables)
        {
            if (_latchedClickDraggable != null)
            {
                // Only the latched click-draggable can be hovered
                if ((clickDraggable == _latchedClickDraggable) && clickDraggable.InteractionArea.Contains(mousePosition))
                {
                    _hovered = clickDraggable;
                    _hovered.IsHovering = true;
                    inputEvent.HoverConsumed = true;
                }
                else
                {
                    clickDraggable.IsHovering = false;
                }
            }
            else if (!inputEvent.HoverConsumed && clickDraggable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(clickDraggable, inputEvent);
            }
            else
            {
                clickDraggable.IsHovering = false;
            }
        }
    }

    void CheckScrollableHover(Vector2 mousePosition, InputEvent inputEvent)
    {
        foreach (var scrollable in _scrollables)
        {
            if (!inputEvent.HoverConsumed && scrollable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(scrollable, inputEvent);
            }
            else
            {
                scrollable.IsHovering = false;
            }
        }
    }

    void CheckRemainingHoverables(Vector2 mousePosition, InputEvent inputEvent)
    {
        foreach (var hoverable in _hoverables)
        {
            if (!inputEvent.HoverConsumed && hoverable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(hoverable, inputEvent);
            }
            else
            {
                hoverable.IsHovering = false;
            }
        }
    }

    void ConsumeHover(IHoverable hoverable, InputEvent inputEvent)
    {
        // Only one hoverable can consume the hover event
        // TODO: This doesn't work! Because the scrollable hoverable should still work when hovering over a clickable,
        // it currently doesn't. Need to rethink this.
        if (_hovered != null)
        {
            throw new InvalidOperationException("Attempted to consume hover when it was already consumed.");
        }
        _hovered = hoverable;
        hoverable.IsHovering = true;
        inputEvent.HoverConsumed = true;
    }
}
