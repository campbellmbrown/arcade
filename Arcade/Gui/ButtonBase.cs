using Arcade.Input;
using MonoGame.Extended;

namespace Arcade.Gui;

public abstract class ButtonBase : Widget, IClickable
{
    public RectangleF InteractionArea => new(Position, new Size2(Width, Height));
    public ClickEvent InputEvent { get; } = new();
}
