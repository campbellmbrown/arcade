using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class HorizontalStack : Stack
{
    public int? FixedHeight { get; set; } = null;

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        base.Update(position, availableWidth, availableHeight);

        int widthOffset = 0;
        foreach (var widget in Widgets)
        {
            int availableWidgetWidth = widget.MeasureWidth();
            widget.Update(Position + new Vector2(widthOffset, 0), availableWidgetWidth, Height);
            widthOffset += widget.OccupiedWidth + Spacing;
        }
    }

    protected override int IntrinsicWidth()
    {
        return Widgets.Count > 0 ? Widgets.Sum(w => w.MeasureWidth()) + (Widgets.Count - 1) * Spacing : 0;
    }

    protected override int IntrinsicHeight()
    {
        return FixedHeight ?? (Widgets.Count > 0 ? Widgets.Max(w => w.MeasureHeight()) : 0);
    }
}
