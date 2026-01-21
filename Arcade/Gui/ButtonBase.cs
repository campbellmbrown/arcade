using Arcade.Input;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Gui;

public abstract class ButtonBase : Widget, IClickable
{
    public RectangleF InteractionArea => new(Position, new Size2(Width, Height));
    public ClickEvent InputEvent { get; } = new();

    public bool IsCheckable { get; set; } = false;
    public bool IsChecked { get; private set; } = false;

    public Color HoverBackgroundColor { get; set; } = Color.White * 0.25f;
    public Color SelectedBackgroundColor { get; set; } = Color.White * 0.5f;

    public event Action<bool>? CheckedChanged;

    protected ButtonBase()
    {
        InputEvent.Released += OnRelease;
    }

    public void Check(bool raiseEvent = true)
    {
        ChangeChecked(true, raiseEvent);
    }

    public void Uncheck(bool raiseEvent = true)
    {
        ChangeChecked(false, raiseEvent);
    }

    public void ChangeChecked(bool isChecked, bool raiseEvent = true)
    {
        if (!IsCheckable)
        {
            return;
        }
        if (IsChecked != isChecked)
        {
            IsChecked = isChecked;
            if (raiseEvent)
            {
                CheckedChanged?.Invoke(IsChecked);
            }
        }
    }

    public override void Draw(IRenderer renderer)
    {
        // Hovered Latched Checked Color
        // ===================================
        // false   false   false   Transparent
        // false   false   true    Selected
        // false   true    false   Hover
        // false   true    true    Hover
        // true    false   false   Hover
        // true    false   true    Hover
        // true    true    false   Selected
        // true    true    true    Selected

        Color backgroundColor;
        if (InputEvent.IsHovered)
        {
            backgroundColor = InputEvent.IsLatched ? SelectedBackgroundColor : HoverBackgroundColor;
        }
        else if (InputEvent.IsLatched)
        {
            backgroundColor = SelectedBackgroundColor;
        }
        else
        {
            backgroundColor = IsChecked ? SelectedBackgroundColor : Color.Transparent;
        }

        renderer.SpriteBatch.FillRectangle(InteractionArea, backgroundColor);

        base.Draw(renderer);
    }

    void OnRelease()
    {
        if (IsCheckable)
        {
            ChangeChecked(!IsChecked);
        }
    }
}
