using Arcade.Input;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Gui;

public interface IButton : IClickable
{
    bool IsCheckable { get; set; }
    bool IsChecked { get; set; }

    Color HoverBackgroundColor { get; set; }
    Color SelectedBackgroundColor { get; set; }

    event Action<bool>? CheckedChanged;
}

public abstract class ButtonBase : Widget, IButton
{
    public RectangleF InteractionArea => new(Position, new Size2(Width, Height));
    public ClickEvent InputEvent { get; } = new();

    public bool IsCheckable { get; set; } = false;

    bool _isChecked = false;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                CheckedChanged?.Invoke(_isChecked);
            }
        }
    }

    public Color HoverBackgroundColor { get; set; } = Color.White * 0.25f;
    public Color SelectedBackgroundColor { get; set; } = Color.White * 0.5f;

    public event Action<bool>? CheckedChanged;

    protected ButtonBase()
    {
        InputEvent.Released += OnRelease;
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
            IsChecked = !IsChecked;
        }
    }
}
