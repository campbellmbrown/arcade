using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Arcade.Input;

public interface IClickDraggable
{
    RectangleF ClickArea { get; }
    void OnLatch();
    void OnDrag(Vector2 position);
    void OnRelease();
}

public interface IClickable
{
    RectangleF ClickArea { get; }
    void OnClick();
}

public interface IScrollable
{
    RectangleF ScrollArea { get; }
    void OnScroll(int delta);
}

public interface IInputService<TControl> : IFrameTickable where TControl : Enum
{
    void RegisterControl(TControl control, Keys key);
    void RegisterHeldKey(TControl control, Action action);
    void RegisterSingleShotKey(TControl control, Action action);
    void RegisterLeftClickDraggable(IClickDraggable clickDraggable);
    void RegisterLeftClickSingleShot(IClickable clickable);
    void RegisterScrollable(IScrollable scrollable);
    void RegisterPan(Action<Vector2>? onPanStart, Action<Vector2>? onPan, Action<Vector2>? onPanEnd);
}

public class InputService<TControl>(ILayerView layerView) : IInputService<TControl> where TControl : Enum
{
    const float ZOOM_FACTOR = 1.2f;
    const float INVERSE_ZOOM_FACTOR = 1 / ZOOM_FACTOR;

    record HeldInput(TControl Control, Keys Key, Action Action);

    record SingleShotInput(TControl Control, Keys Key, Action Action)
    {
        public bool IsKeyHeldDown { get; set; } = true;
    }

    readonly Dictionary<TControl, Keys> _controlKeys = [];
    readonly List<HeldInput> _heldKeys = [];
    readonly List<SingleShotInput> _singleShotInputs = [];
    readonly List<IClickDraggable> _clickDraggables = [];
    readonly List<IClickable> _clickables = [];
    readonly List<IScrollable> _scrollables = [];

    Action<Vector2>? _onPanStart;
    Action<Vector2>? _onPan;
    Action<Vector2>? _onPanEnd;

    int _previousScrollValue = 0;

    public void RegisterControl(TControl control, Keys key)
    {
        _controlKeys[control] = key;
    }

    public void RegisterHeldKey(TControl control, Action action)
    {
        if (!_controlKeys.TryGetValue(control, out Keys key))
        {
            throw new ArgumentException($"Control {control} has not been registered.");
        }

        _heldKeys.Add(new HeldInput(control, key, action));
    }

    public void RegisterSingleShotKey(TControl control, Action action)
    {
        if (!_controlKeys.TryGetValue(control, out Keys key))
        {
            throw new ArgumentException($"Control {control} has not been registered.");
        }

        _singleShotInputs.Add(new SingleShotInput(control, key, action));
    }

    public void RegisterLeftClickDraggable(IClickDraggable clickDraggable)
    {
        _clickDraggables.Add(clickDraggable);
    }

    public void RegisterLeftClickSingleShot(IClickable clickable)
    {
        _clickables.Add(clickable);
    }

    public void RegisterScrollable(IScrollable scrollable)
    {
        _scrollables.Add(scrollable);
    }

    public void RegisterPan(Action<Vector2>? onPanStart, Action<Vector2>? onPan, Action<Vector2>? onPanEnd)
    {
        if ((onPanStart == null) && (onPan == null) && (onPanEnd == null))
        {
            throw new ArgumentException("All pan callbacks cannot be null.");
        }
        if (onPanStart != null)
        {
            if (_onPanStart != null)
            {
                throw new InvalidOperationException("Pan start callback has already been registered.");
            }
            _onPanStart = onPanStart;
        }
        if (onPan != null)
        {
            if (_onPan != null)
            {
                throw new InvalidOperationException("Pan callback has already been registered.");
            }
            _onPan = onPan;
        }
        if (onPanEnd != null)
        {
            if (_onPanEnd != null)
            {
                throw new InvalidOperationException("Pan end callback has already been registered.");
            }
            _onPanEnd = onPanEnd;
        }
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        foreach (var singleShot in _singleShotInputs)
        {
            if (keyboardState.IsKeyDown(singleShot.Key))
            {
                if (!singleShot.IsKeyHeldDown)
                {
                    singleShot.Action();
                    singleShot.IsKeyHeldDown = true;
                }
            }
            else
            {
                singleShot.IsKeyHeldDown = false;
            }
        }

        foreach (var heldKey in _heldKeys)
        {
            if (keyboardState.IsKeyDown(heldKey.Key))
            {
                heldKey.Action();
            }
        }

        var mouseState = Mouse.GetState();
        HandleLeftClick(mouseState.LeftButton);
        HandleMiddleClick(mouseState.MiddleButton);
        HandleMouseWheel(mouseState.ScrollWheelValue);
    }

    IClickable? _pressedClickable;
    IClickDraggable? _latchedClickDraggable;
    ButtonState _previousLeftButtonState = ButtonState.Released;

    void HandleLeftClick(ButtonState leftButtonState)
    {
        if (leftButtonState == ButtonState.Pressed)
        {
            if (_previousLeftButtonState == ButtonState.Released)
            {
                foreach (var leftClick in _clickables)
                {
                    if (leftClick.ClickArea.Contains(layerView.MousePosition))
                    {
                        _pressedClickable = leftClick;
                        break;
                    }
                    _pressedClickable = null; // Nothing was clicked.
                }

                // Clicked for the first time. Check if we clicked something.
                foreach (var leftClickDraggable in _clickDraggables)
                {
                    if (leftClickDraggable.ClickArea.Contains(layerView.MousePosition))
                    {
                        // We clicked a draggable.
                        leftClickDraggable.OnLatch();
                        _latchedClickDraggable = leftClickDraggable;
                        break;
                    }

                    // We clicked nothing.
                    _latchedClickDraggable = null;
                }
            }

            // If we are latched onto a draggable, drag it.
            _latchedClickDraggable?.OnDrag(layerView.MousePosition);
        }
        else
        {
            // Check for release
            if (_previousLeftButtonState == ButtonState.Pressed)
            {
                if ((_pressedClickable != null) && _pressedClickable.ClickArea.Contains(layerView.MousePosition))
                {
                    // We clicked something and released the mouse button.
                    _pressedClickable.OnClick();
                }
                _pressedClickable = null;

                if (_latchedClickDraggable != null)
                {
                    // We were latched onto a draggable and released the mouse button.
                    _latchedClickDraggable.OnRelease();
                    _latchedClickDraggable = null;
                }
            }
        }

        _previousLeftButtonState = leftButtonState;
    }

    ButtonState _previousMiddleButtonState = ButtonState.Released;
    Vector2 _panPosition = Vector2.Zero;

    void HandleMiddleClick(ButtonState middleButtonState)
    {
        if (middleButtonState == ButtonState.Pressed)
        {
            if (_previousMiddleButtonState == ButtonState.Released)
            {
                _panPosition = layerView.MousePosition;
                _onPanStart?.Invoke(_panPosition);
            }
            var panDelta = _panPosition - layerView.MousePosition;
            if (panDelta != Vector2.Zero)
            {
                _onPan?.Invoke(panDelta);
            }
        }
        else
        {
            if (_previousMiddleButtonState == ButtonState.Pressed)
            {
                var panDelta = _panPosition - layerView.MousePosition;
                _onPanEnd?.Invoke(panDelta);
            }
        }

        _previousMiddleButtonState = middleButtonState;
    }

    void HandleMouseWheel(int currentScroll)
    {
        int delta = currentScroll - _previousScrollValue;
        _previousScrollValue = currentScroll;

        foreach (var scrollable in _scrollables)
        {
            if (scrollable.ScrollArea.Contains(layerView.MousePosition))
            {
                scrollable.OnScroll(delta);
                return;
            }
        }

        if (delta > 0)
        {
            // TODO: register these as non-area-specific scrollables that default
            // when nothing else is scrolled over.
            layerView.ZoomAtMouse(ZOOM_FACTOR);
        }
        else if (delta < 0)
        {
            layerView.ZoomAtMouse(INVERSE_ZOOM_FACTOR);
        }
    }
}
