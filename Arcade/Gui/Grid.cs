using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Gui;

public enum GridSizeType
{
    Fixed,
    Auto,
    Stretch,
}

public class GridSize
{
    public float Value { get; private set; }
    public GridSizeType Type { get; private set; }

    /// <summary>
    /// The size will be determined automatically based on the content.
    /// </summary>
    /// <returns>A GridSize instance representing an automatic size.</returns>
    public static GridSize Auto() => new() { Type = GridSizeType.Auto };

    /// <summary>
    /// The size is fixed to the specified value.
    /// </summary>
    /// <param name="size">The fixed size.</param>
    /// <returns>A GridSize instance representing a fixed size.</returns>
    public static GridSize Fixed(int size) => new() { Type = GridSizeType.Fixed, Value = size };

    /// <summary>
    /// The size will stretch to fill the available space.
    /// </summary>
    /// <remarks>
    /// TODO: support weighted stretching
    /// </remarks>
    /// <returns>A GridSize instance representing a stretch size.</returns>
    public static GridSize Stretch() => new() { Type = GridSizeType.Stretch };
}

public class Grid(List<GridSize> rows, List<GridSize> columns) : Widget
{
    readonly Dictionary<(int Column, int Row), IWidget> _widgets = [];

    public List<GridSize> Rows { get; } = rows;
    public List<GridSize> Columns { get; } = columns;

    readonly List<int> _columnSizes = [.. Enumerable.Repeat(0, columns.Count)];
    readonly List<int> _rowSizes = [.. Enumerable.Repeat(0, rows.Count)];

    public override int GetContentWidth()
    {
        int total = 0;
        for (int column = 0; column < Columns.Count; column++)
        {
            if (Columns[column].Type == GridSizeType.Fixed)
            {
                total += (int)Columns[column].Value;
            }
            else if (Columns[column].Type == GridSizeType.Auto)
            {
                total += GetColumnMinWidth(column);
            }
        }
        return total;
    }

    public override int GetContentHeight()
    {
        int total = 0;
        for (int row = 0; row < Rows.Count; row++)
        {
            if (Rows[row].Type == GridSizeType.Fixed)
            {
                total += (int)Rows[row].Value;
            }
            else if (Rows[row].Type == GridSizeType.Auto)
            {
                total += GetRowMinHeight(row);
            }
        }
        return total;
    }

    public void AddWidget(IWidget widget, int row, int column)
    {
        if ((row < 0) || (row >= Rows.Count))
        {
            throw new ArgumentOutOfRangeException(nameof(row));
        }
        if ((column < 0) || (column >= Columns.Count))
        {
            throw new ArgumentOutOfRangeException(nameof(column));
        }
        _widgets[(row, column)] = widget;
    }

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        // If any of the columns are stretch, the grid takes all available width.
        // TODO: move the setting of Width and Height to the base Widget class
        Width = (Columns.Any(c => c.Type == GridSizeType.Stretch) ? availableWidth : MeasureWidth()) - MarginLeft - MarginRight;
        Height = (Rows.Any(r => r.Type == GridSizeType.Stretch) ? availableHeight : MeasureHeight()) - MarginTop - MarginBottom;
        base.Update(position, availableWidth, availableHeight);

        CalculateFinalColumnSizes(Width);
        CalculateFinalRowSizes(Height);

        int y = 0;
        for (int row = 0; row < Rows.Count; row++)
        {
            int x = 0;
            for (int column = 0; column < Columns.Count; column++)
            {
                if (_widgets.TryGetValue((row, column), out var widget))
                {
                    widget.Update(Position + new Vector2(x, y), _columnSizes[column], _rowSizes[row]);
                }
                x += _columnSizes[column];
            }
            y += _rowSizes[row];
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
        // TODO: Remove debug drawing
        var totalWidth = _columnSizes.Sum();
        var totalHeight = _rowSizes.Sum();
        int width = 0;
        for (int column = 0; column <= Columns.Count; column++)
        {
            renderer.SpriteBatch.DrawLine(Position + new Vector2(width, 0), Position + new Vector2(width, totalHeight), Color.Green * 0.25f, 4);
            if (column < Columns.Count)
            {
                width += _columnSizes[column];
            }
        }
        int height = 0;
        for (int row = 0; row <= Rows.Count; row++)
        {
            renderer.SpriteBatch.DrawLine(Position + new Vector2(0, height), Position + new Vector2(totalWidth, height), Color.Green * 0.25f, 4);
            if (row < Rows.Count)
            {
                height += _rowSizes[row];
            }
        }
        foreach (var widget in _widgets.Values)
        {
            widget.Draw(renderer);
        }
        base.Draw(renderer);
    }

    int GetColumnMinWidth(int column)
    {
        int maxColumnWidth = 0;
        for (int row = 0; row < Rows.Count; row++)
        {
            if (_widgets.TryGetValue((row, column), out var widget))
            {
                maxColumnWidth = Math.Max(maxColumnWidth, widget.MeasureWidth());
            }
        }
        return maxColumnWidth;
    }

    int GetRowMinHeight(int row)
    {
        int maxRowHeight = 0;
        for (int column = 0; column < Columns.Count; column++)
        {
            if (_widgets.TryGetValue((row, column), out var widget))
            {
                maxRowHeight = Math.Max(maxRowHeight, widget.MeasureHeight());
            }
        }
        return maxRowHeight;
    }

    void CalculateFinalColumnSizes(int availableWidth)
    {
        int used = 0;
        int stretchCount = 0;

        for (int column = 0; column < Columns.Count; column++)
        {
            if (Columns[column].Type == GridSizeType.Fixed)
            {
                int size = (int)Columns[column].Value;
                _columnSizes[column] = size;
                used += size;
            }
            else if (Columns[column].Type == GridSizeType.Auto)
            {
                int size = GetColumnMinWidth(column);
                _columnSizes[column] = size;
                used += size;
            }
            else
            {
                stretchCount++; // Stretch will be calculated later
            }
        }

        int remaining = Math.Max(0, availableWidth - used);
        int perStretch = stretchCount > 0 ? remaining / stretchCount : 0;

        for (int column = 0; column < Columns.Count; column++)
        {
            if (Columns[column].Type == GridSizeType.Stretch)
            {
                _columnSizes[column] = perStretch;
            }
        }
    }

    void CalculateFinalRowSizes(int availableHeight)
    {
        int used = 0;
        int stretchCount = 0;

        for (int row = 0; row < Rows.Count; row++)
        {
            if (Rows[row].Type == GridSizeType.Fixed)
            {
                int size = (int)Rows[row].Value;
                _rowSizes[row] = size;
                used += size;
            }
            else if (Rows[row].Type == GridSizeType.Auto)
            {
                int size = GetRowMinHeight(row);
                _rowSizes[row] = size;
                used += size;
            }
            else
            {
                stretchCount++; // Stretch will be calculated later
            }
        }

        int remaining = Math.Max(0, availableHeight - used);
        int perStretch = stretchCount > 0 ? remaining / stretchCount : 0;

        for (int row = 0; row < Rows.Count; row++)
        {
            if (Rows[row].Type == GridSizeType.Stretch)
            {
                _rowSizes[row] = perStretch;
            }
        }
    }
}
