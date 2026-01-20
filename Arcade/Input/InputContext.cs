using System.Diagnostics;
using Arcade.Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Input;

public class HoverEvent
{
    public bool IsHovering { get; set; }
}

public class ClickEvent : HoverEvent
{
    /// <summary>
    /// The current latched state of the clickable.
    /// </summary>
    /// <remarks>
    /// This should only be set by the input system.
    /// </remarks>
    /// <value>True if the clickable is latched, false otherwise.</value>
    public bool IsLatched { get; set; }

    /// <summary>
    /// Raised when the clickable is released.
    /// </summary>
    public event Action? Latched;

    /// <summary>
    /// Raised when the clickable is released.
    /// </summary>
    public event Action? Released;

    public void Latch()
    {
        IsLatched = true;
        Latched?.Invoke();
    }

    public void Release(bool fireEvent = true)
    {
        IsLatched = false;
        if (fireEvent)
        {
            Released?.Invoke();
        }
    }
}

public class ClickDragEvent : ClickEvent
{
    /// <summary>
    /// Raised when the clickable is dragged.
    /// </summary>
    public event Action<Vector2>? Dragged;

    public void Drag(Vector2 position)
    {
        Dragged?.Invoke(position);
    }
}

public class ScrollEvent : HoverEvent
{
    /// <summary>
    /// Raised when the scrollable is scrolled.
    /// </summary>
    public event Action<int>? Scrolled;

    public void Scroll(int delta)
    {
        Scrolled?.Invoke(delta);
    }
}

public interface IClickable : IInteractive<ClickEvent>;
public interface IClickDraggable : IInteractive<ClickDragEvent>;
public interface IScrollable : IInteractive<ScrollEvent>;
public interface IHoverable : IInteractive<HoverEvent>;

public interface IInteractive<out E> where E : HoverEvent
{
    RectangleF InteractionArea { get; }
    E InputEvent { get; }
}

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

    // Only one of the clickables, click-draggables, or hoverables can be hovered at a time
    IInteractive<HoverEvent>? _hovered = null;

    // Only one scrollable can be hovered at a time, independent of the other hoverables
    IScrollable? _hoveredScrollable = null;

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
            _latchedClickDraggable.InputEvent.Latch();
            _latchedClickDraggable.InputEvent.Drag(layerView.MousePosition);
            inputEvent.LeftClickConsumed = true;
        }
        else if (_hovered is IClickable clickable)
        {
            _latchedClickable = clickable;
            _latchedClickable.InputEvent.Latch();
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
            _latchedClickDraggable.InputEvent.Drag(layerView.MousePosition);
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
            _latchedClickable.InputEvent.Release(fireEvent: _latchedClickable.InputEvent.IsHovering);
            _latchedClickable = null;
            inputEvent.LeftClickConsumed = true;
        }
        else if (_latchedClickDraggable != null)
        {
            _latchedClickDraggable.InputEvent.Release();
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

        if (_hoveredScrollable != null)
        {
            _hoveredScrollable.InputEvent.Scroll(delta);
            inputEvent.ScrollConsumed = true;
        }
    }

    public void HandleHover(InputEvent inputEvent)
    {
        _hovered = null;
        _hoveredScrollable = null;
        var mousePosition = layerView.MousePosition;

        // First, check the scrollables. Only one of these can be hovered (if the hover event wasn't already consumed)
        // This does not consume the hover event, so other hoverables can still be checked
        CheckScrollableHover(mousePosition, inputEvent);

        // If there is a latched clickable, only it can be hovered. This consumes the hover event so no other
        // hoverables are checked
        CheckLatchedHover(_latchedClickable, mousePosition, inputEvent);
        CheckLatchedHover(_latchedClickDraggable, mousePosition, inputEvent);

        // Then check the other hoverables. The latched hoverable (if any) has already been checked, so skip it here
        CheckHoverables(_leftClickSingleShots, mousePosition, inputEvent, skip: _latchedClickable);
        CheckHoverables(_leftClickDraggables, mousePosition, inputEvent, skip: _latchedClickDraggable);
        CheckHoverables(_hoverables, mousePosition, inputEvent);

        // Finally, consume the hover if the scrollable that we checked earlier is hovered
        if (_hoveredScrollable != null)
        {
            inputEvent.HoverConsumed = true;
        }
    }

    void CheckScrollableHover(Vector2 mousePosition, InputEvent inputEvent)
    {
        foreach (var scrollable in _scrollables)
        {
            // Because we don't consume the hover event for scrollables, we need to make sure only one scrollable
            // can be hovered at a time, hence the _hoveredScrollable check.
            if ((_hoveredScrollable == null) && !inputEvent.HoverConsumed && scrollable.InteractionArea.Contains(mousePosition))
            {
                _hoveredScrollable = scrollable;
                _hoveredScrollable.InputEvent.IsHovering = true;
                // Don't consume the hover just yet, because other non-scrollable hoverables may need to be checked
            }
            else
            {
                scrollable.InputEvent.IsHovering = false;
            }
        }
    }

    void CheckLatchedHover(IInteractive<HoverEvent>? hoverable, Vector2 mousePosition, InputEvent inputEvent)
    {
        if (inputEvent.HoverConsumed && (hoverable != null))
        {
            if (hoverable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(hoverable, inputEvent);
            }
            else
            {
                hoverable.InputEvent.IsHovering = false;
            }
            inputEvent.HoverConsumed = true; // Only the latched hoverable can be hovered
        }
    }

    void CheckHoverables(
        IEnumerable<IInteractive<HoverEvent>> hoverables,
        Vector2 mousePosition,
        InputEvent inputEvent,
        IInteractive<HoverEvent>? skip = null
    )
    {
        foreach (var hoverable in hoverables)
        {
            if (hoverable == skip)
            {
                continue;
            }
            if (!inputEvent.HoverConsumed && hoverable.InteractionArea.Contains(mousePosition))
            {
                ConsumeHover(hoverable, inputEvent);
            }
            else
            {
                hoverable.InputEvent.IsHovering = false;
            }
        }
    }

    void ConsumeHover(IInteractive<HoverEvent> hoverable, InputEvent inputEvent)
    {
        Debug.Assert(_hovered == null, "Only one hoverable can be hovered at a time.");
        _hovered = hoverable;
        hoverable.InputEvent.IsHovering = true;
        inputEvent.HoverConsumed = true;
    }
}
