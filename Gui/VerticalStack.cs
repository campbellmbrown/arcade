using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class VerticalStack : Stack
{
    public int? FixedWidth { get; init; }

    public override void Update(Vector2 position)
    {
        if (FixedWidth.HasValue)
        {
            Width = FixedWidth.Value;
        }
        else
        {
            Width = (Widgets.Count > 0 ? Widgets.Max(w => w.Width) : 0) + 2 * Margin;
        }
        Height = (Widgets.Count > 0 ? Widgets.Sum(w => w.Height) + (Widgets.Count - 1) * Spacing : 0) + 2 * Margin;
        base.Update(position);

        int heightOffset = Margin;
        foreach (var widget in Widgets)
        {
            int availableHeight = widget.Height;
            widget.Update(Position + new Vector2(Margin, heightOffset), Width, availableHeight);
            heightOffset += widget.Height + Spacing;
        }
    }
}
