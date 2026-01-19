using Arcade.Input;
using MonoGame.Extended;

namespace Arcade.Gui;

public abstract class ButtonBase : Widget, IClickable
{
    protected List<Action> _actions = [];

    public RectangleF InteractionArea => new(Position, new Size2(Width, Height));
    public bool IsHovering { get; set; } = false;
    public bool IsLatched { get; set; } = false;

    // TODO: support latch and release actions separately?
    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

    public void OnLatch()
    {
    }

    public void OnRelease()
    {
        foreach (Action action in _actions)
        {
            action();
        }
    }
}
