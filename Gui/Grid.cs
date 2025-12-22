using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public enum GridUnitType
{
    Fixed,
    Auto,
}

public record GridUnitDimension
{
    public int Size { get; set; }
    public GridUnitType Type { get; set; } = GridUnitType.Auto;
}

public class GridUnit
{
    public List<GridUnitDimension> Dimensions { get; set; }= [];

    public int Count => Dimensions.Count;
    public int TotalSize => Dimensions.Sum(d => d.Size);

    public void Register(int widgetSize, int index)
    {
        if (Dimensions.Count <= index)
        {
            Dimensions.AddRange(Enumerable.Repeat(new GridUnitDimension(), index - Dimensions.Count + 1));
        }
        if (Dimensions[index].Type == GridUnitType.Auto)
        {
            if (widgetSize > Dimensions[index].Size)
            {
                Dimensions[index].Size = widgetSize;
            }
        }
    }

    public void FixSize(int dimension, int size)
    {
        Dimensions[dimension].Size = size;
        Dimensions[dimension].Type = GridUnitType.Fixed;
    }
}


public class Grid : Widget
{
    readonly Dictionary<(int Column, int Row), IWidget> _widgets = [];

    public int Spacing { get; init; } = 0;
    public int Margin { get; init; } = 0;

    public GridUnit Rows { get; } = new();
    public GridUnit Columns { get; } = new();

    public int RowCount => Rows.Count;
    public int ColumnCount => Columns.Count;

    public void FixRowHeight(int row, int size) => Rows.FixSize(row, size);
    public void FixColumnWidth(int column, int size) => Columns.FixSize(column, size);

    public void AddWidget(IWidget widget, int column, int row)
    {
        _widgets[(column, row)] = widget;
        Rows.Register(widget.Height, row);
        Columns.Register(widget.Width, column);
    }

    public override void Update(Vector2 position)
    {
        Width = Columns.TotalSize + (2 * Margin) + ((ColumnCount - 1) * Spacing);
        Height = Rows.TotalSize + (2 * Margin) + ((RowCount - 1) * Spacing);
        base.Update(position);

        int heightOffset = Margin;
        for (int row = 0; row < Rows.Count; row++)
        {
            int widthOffset = Margin;
            int availableHeight = Rows.Dimensions[row].Size;
            for (int column = 0; column < Columns.Count; column++)
            {
                int availableWidth = Columns.Dimensions[column].Size;
                if (_widgets.TryGetValue((column, row), out var widget))
                {
                    widget.Update(Position + new Vector2(widthOffset, heightOffset), availableWidth, availableHeight);
                }
                widthOffset += availableWidth + Spacing;
            }
            heightOffset += availableHeight + Spacing;
        }
    }

    public override void FrameTick(IFrameTickService frameTickService)
    {
        foreach (var widget in _widgets.Values)
        {
            widget.FrameTick(frameTickService);
        }
    }

    public override void Draw(IRenderer renderer)
    {
        foreach (var widget in _widgets.Values)
        {
            widget.Draw(renderer);
        }
    }
}
