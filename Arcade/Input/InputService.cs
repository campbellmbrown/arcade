using Arcade.Core;
using Microsoft.Xna.Framework.Input;

namespace Arcade.Input;

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

    /// <summary>
    /// Registers a default scrollable that is invoked when none of the contexts handle the scroll (i.e. when no
    /// registered scrollable contains the mouse position).
    /// </summary>
    /// <param name="onScroll">The action to invoke on scroll.</param>
    void RegisterDefaultScrollable(Action<int> onScroll);

    IInputContext Gui { get; }
    IInputContext World { get; }

    bool IsHoveringAnyContext { get; }
}

public class InputService<TControl>(ILayerView guiLayer, ILayerView worldLayer) : IInputService<TControl> where TControl : Enum
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

    Action<int>? _defaultScrollable;

    ButtonState _previousLeftButtonState = ButtonState.Released;
    ButtonState _previousMiddleButtonState = ButtonState.Released;
    int _previousScrollValue = 0;

    public IInputContext Gui { get; } = new InputContext(guiLayer);
    public IInputContext World { get; } = new InputContext(worldLayer);

    public bool IsHoveringAnyContext { get; private set; } = false;

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
        InputEvent inputEvent = new();

        HandleHover(inputEvent); // Must be done first
        HandleLeftClick(mouseState.LeftButton, inputEvent);
        HandleMiddleClick(mouseState.MiddleButton);
        HandleMouseWheel(mouseState.ScrollWheelValue, inputEvent);
    }

    void HandleLeftClick(ButtonState leftButtonState, InputEvent inputEvent)
    {
        if (leftButtonState == ButtonState.Pressed)
        {
            if (_previousLeftButtonState == ButtonState.Released)
            {
                Gui.HandleLeftClickStart(inputEvent);
                World.HandleLeftClickStart(inputEvent);
            }
            else
            {
                Gui.HandleLeftClickHold(inputEvent);
                World.HandleLeftClickHold(inputEvent);
            }
        }
        else if (_previousLeftButtonState == ButtonState.Pressed)
        {
            Gui.HandleLeftClickRelease(inputEvent);
            World.HandleLeftClickRelease(inputEvent);
        }
        _previousLeftButtonState = leftButtonState;
    }

    // TODO: use InputEvent, and move to IInputContext
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

    void HandleMouseWheel(int currentScroll, InputEvent inputEvent)
    {
        int delta = currentScroll - _previousScrollValue;
        if (delta == 0)
        {
            return;
        }
        _previousScrollValue = currentScroll;

        Gui.HandleScroll(delta, inputEvent);
        World.HandleScroll(delta, inputEvent);

        if (!inputEvent.ScrollConsumed)
        {
            _defaultScrollable?.Invoke(delta);
        }
    }

    void HandleHover(InputEvent inputEvent)
    {
        Gui.HandleHover(inputEvent);
        World.HandleHover(inputEvent);
        IsHoveringAnyContext = inputEvent.HoverConsumed;
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
