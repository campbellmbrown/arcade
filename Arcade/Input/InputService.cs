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

public interface IInputServiceGeneric
{
    void TearDown();
}

// TODO: move more interfaces to `IInputServiceGeneric` if needed.
public interface IInputService<TControl> : IInputServiceGeneric, IFrameTickable where TControl : Enum
{
    void RegisterControl(TControl control, Keys key);
    void RegisterHeldKey(TControl control, Action action);
    void RegisterSingleShotKey(TControl control, Action action);
    void RegisterLeftClickDraggable(IClickDraggable clickDraggable);
    void RegisterLeftClickSingleShot(IClickable clickable);

    /// <summary>
    /// Registers a default scrollable that is invoked when none of the contexts handle the scroll (i.e. when no
    /// registered scrollable contains the mouse position).
    /// </summary>
    /// <param name="onScroll">The action to invoke on scroll.</param>
    void RegisterDefaultScrollable(Action<int> onScroll);

    IInputContext Gui { get; }
    IInputContext World { get; }
}

public class InputService<TControl>(ILayerView guiLayerView, ILayerView worldLayerView) : IInputService<TControl> where TControl : Enum
{
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

    Action<int>? _defaultScrollable;

    int _previousScrollValue = 0;

    public IInputContext Gui { get; } = new InputContext(guiLayerView);
    public IInputContext World { get; } = new InputContext(worldLayerView);

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

    public void RegisterDefaultScrollable(Action<int> onScroll)
    {
        if (_defaultScrollable != null)
        {
            throw new InvalidOperationException("Default scrollable has already been registered.");
        }
        _defaultScrollable = onScroll;
    }

    public void TearDown()
    {
        _heldKeys.Clear();
        _singleShotInputs.Clear();
        _clickDraggables.Clear();
        _clickables.Clear();
        _defaultScrollable = null;
        Gui.TearDown();
        World.TearDown();
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
                    if (leftClick.ClickArea.Contains(guiLayerView.MousePosition))
                    {
                        _pressedClickable = leftClick;
                        break;
                    }
                    _pressedClickable = null; // Nothing was clicked.
                }

                // Clicked for the first time. Check if we clicked something.
                foreach (var leftClickDraggable in _clickDraggables)
                {
                    if (leftClickDraggable.ClickArea.Contains(guiLayerView.MousePosition))
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
            _latchedClickDraggable?.OnDrag(guiLayerView.MousePosition);
        }
        else
        {
            // Check for release
            if (_previousLeftButtonState == ButtonState.Pressed)
            {
                if ((_pressedClickable != null) && _pressedClickable.ClickArea.Contains(guiLayerView.MousePosition))
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

    void HandleMiddleClick(ButtonState middleButtonState)
    {
        if (middleButtonState == ButtonState.Pressed)
        {
            if (_previousMiddleButtonState == ButtonState.Released)
            {
                Gui.HandlePanStart();
                World.HandlePanStart();
            }
            else
            {
                Gui.HandlePan();
                World.HandlePan();
            }
        }
        else if (_previousMiddleButtonState == ButtonState.Pressed)
        {
            Gui.HandlePanEnd();
            World.HandlePanEnd();
        }

        _previousMiddleButtonState = middleButtonState;
    }

    void HandleMouseWheel(int currentScroll)
    {
        int delta = currentScroll - _previousScrollValue;
        if (delta == 0)
        {
            return;
        }
        _previousScrollValue = currentScroll;

        if (Gui.HandleScroll(delta))
        {
            return;
        }
        if (World.HandleScroll(delta))
        {
            return;
        }

        _defaultScrollable?.Invoke(delta);
    }
}
