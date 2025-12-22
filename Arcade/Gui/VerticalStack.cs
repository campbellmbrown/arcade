using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class VerticalStack : Stack
{
    public int? FixedWidth { get; init; }

    public override int GetContentWidth()
    {
        return FixedWidth ?? (Widgets.Count > 0 ? Widgets.Max(w => w.GetContentWidth()) : 0) + (2 * Margin);
    }

    public override int GetContentHeight()
    {
        return (Widgets.Count > 0 ? Widgets.Sum(w => w.GetContentHeight()) + (Widgets.Count - 1) * Spacing : 0) + (2 * Margin);
    }

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        Width = GetContentWidth();
        Height = GetContentHeight();
        base.Update(position, availableWidth, availableHeight);

        int heightOffset = Margin;
        foreach (var widget in Widgets)
        {
            int availableWidgetHeight = widget.GetContentHeight();
            widget.Update(Position + new Vector2(Margin, heightOffset), Width - 2 * Margin, availableWidgetHeight);
            heightOffset += widget.Height + Spacing;
        }
    }
}
