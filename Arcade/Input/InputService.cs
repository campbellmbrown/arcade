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

[Flags]
public enum KeyModifiers
{
    None = 0,
    Control = 1 << 0,
    Alt = 1 << 1,
    Shift = 1 << 2,
}

public static class KeyModifierExtensions
{
    public static int Count(this KeyModifiers self)
    {
        int count = 0;
        if (self.HasFlag(KeyModifiers.Control))
        {
            count++;
        }
        if (self.HasFlag(KeyModifiers.Alt))
        {
            count++;
        }
        if (self.HasFlag(KeyModifiers.Shift))
        {
            count++;
        }
        return count;
    }
}

// TODO: move more interfaces to `IInputServiceGeneric` if needed.
public interface IInputService<TControl> : IInputServiceGeneric, IFrameTickable where TControl : Enum
{
    void RegisterControl(TControl control, Keys key, KeyModifiers modifiers = KeyModifiers.None);
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
    public record KeyBinding(Keys Key, KeyModifiers Modifiers);

    record HeldInput(TControl Control, KeyBinding KeyBinding, Action Action);

    record SingleShotInput(TControl Control, KeyBinding KeyBinding, Action Action)
    {
        public bool IsKeyHeldDown { get; set; } = true;
    }

    readonly Dictionary<TControl, KeyBinding> _controlKeys = [];
    readonly List<HeldInput> _heldKeys = [];
    readonly List<SingleShotInput> _singleShotInputs = [];
    readonly List<IClickDraggable> _clickDraggables = [];
    readonly List<IClickable> _clickables = [];

    Action<int>? _defaultScrollable;

    int _previousScrollValue = 0;

    public IInputContext Gui { get; } = new InputContext(guiLayerView);
    public IInputContext World { get; } = new InputContext(worldLayerView);

    public void RegisterControl(TControl control, Keys key, KeyModifiers modifiers = KeyModifiers.None)
    {
        _controlKeys[control] = new KeyBinding(key, modifiers);
    }

    public void RegisterHeldKey(TControl control, Action action)
    {
        if (!_controlKeys.TryGetValue(control, out var keyBinding))
        {
            throw new ArgumentException($"Control {control} has not been registered.");
        }

        // Sort by the number of modifiers, descending, so that more specific bindings are checked first.
        int index = _heldKeys.FindIndex(input => input.KeyBinding.Modifiers.Count() < keyBinding.Modifiers.Count());
        if (index >= 0)
        {
            _heldKeys.Insert(index, new HeldInput(control, keyBinding, action));
        }
        else
        {
            _heldKeys.Add(new HeldInput(control, keyBinding, action));
        }
    }

    public void RegisterSingleShotKey(TControl control, Action action)
    {
        if (!_controlKeys.TryGetValue(control, out var keyBinding))
        {
            throw new ArgumentException($"Control {control} has not been registered.");
        }

        // Sort by the number of modifiers, descending, so that more specific bindings are checked first.
        int index = _singleShotInputs.FindIndex(input => input.KeyBinding.Modifiers.Count() < keyBinding.Modifiers.Count());
        if (index >= 0)
        {
            _singleShotInputs.Insert(index, new SingleShotInput(control, keyBinding, action));
        }
        else
        {
            _singleShotInputs.Add(new SingleShotInput(control, keyBinding, action));
        }
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
            if (keyboardState.IsKeyDown(singleShot.KeyBinding.Key) && AreModifiersPressed(singleShot.KeyBinding.Modifiers, keyboardState))
            {
                if (!singleShot.IsKeyHeldDown)
                {
                    singleShot.Action();
                    singleShot.IsKeyHeldDown = true;
                    break; // Fire only first match
                }
            }
            else
            {
                singleShot.IsKeyHeldDown = false;
            }
        }

        foreach (var heldKey in _heldKeys)
        {
            if (keyboardState.IsKeyDown(heldKey.KeyBinding.Key) && AreModifiersPressed(heldKey.KeyBinding.Modifiers, keyboardState))
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

    static bool AreModifiersPressed(KeyModifiers required, KeyboardState keyboard)
    {
        bool ctrlPressed = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
        bool altPressed = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
        bool shiftPressed = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

        return (ctrlPressed == required.HasFlag(KeyModifiers.Control)) &&
               (altPressed == required.HasFlag(KeyModifiers.Alt)) &&
               (shiftPressed == required.HasFlag(KeyModifiers.Shift));
    }
}
