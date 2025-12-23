using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class VerticalStack : Stack
{
    public int? FixedWidth { get; init; }

    public override int GetContentWidth()
    {
        return FixedWidth ?? (Widgets.Count > 0 ? Widgets.Max(w => w.GetContentWidth()) : 0) + MarginLeft + MarginRight;
    }

    public override int GetContentHeight()
    {
        return (Widgets.Count > 0 ? Widgets.Sum(w => w.GetContentHeight()) + (Widgets.Count - 1) * Spacing : 0) + MarginTop + MarginBottom;
    }

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        Width = GetContentWidth() - MarginLeft - MarginRight;
        Height = GetContentHeight() - MarginTop - MarginBottom;
        base.Update(position, availableWidth, availableHeight);

        int heightOffset = 0;
        foreach (var widget in Widgets)
        {
            int availableWidgetHeight = widget.GetContentHeight();
            widget.Update(Position + new Vector2(0, heightOffset), Width, availableWidgetHeight);
            heightOffset += widget.GetContentHeight() + Spacing;
        }
    }
}
