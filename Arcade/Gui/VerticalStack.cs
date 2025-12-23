using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class VerticalStack : Stack
{
    public int? FixedWidth { get; set; } = null;

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        base.Update(position, availableWidth, availableHeight);

        int heightOffset = 0;
        foreach (var widget in Widgets)
        {
            int availableWidgetHeight = widget.MeasureHeight();
            widget.Update(Position + new Vector2(0, heightOffset), Width, availableWidgetHeight);
            heightOffset += widget.OccupiedHeight + Spacing;
        }
    }

    protected override int IntrinsicWidth()
    {
        return FixedWidth ?? (Widgets.Count > 0 ? Widgets.Max(w => w.MeasureWidth()) : 0);
    }

    protected override int IntrinsicHeight()
    {
        return Widgets.Count > 0 ? Widgets.Sum(w => w.MeasureHeight()) + (Widgets.Count - 1) * Spacing : 0;
    }
}
