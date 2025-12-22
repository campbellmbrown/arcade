using System.Diagnostics;
using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public interface IWidget : IVisual, IFrameTickable
{
    /// <summary>
    /// The width of the widget in pixels, including any layout spacing (e.g. due to padding or alignment).
    /// </summary>
    /// <value>The width in pixels.</value>
    int Width { get; }

    /// <summary>
    /// The height of the widget in pixels, including any layout spacing (e.g. due to padding or alignment).
    /// </summary>
    /// <value>The height in pixels.</value>
    int Height { get; }

    /// <summary>
    /// The position of the widget in the view.
    /// </summary>
    /// <value>The position.</value>
    Vector2 Position { get; }

    /// <summary>
    /// The alignment of the widget within its allocated space.
    /// </summary>
    /// <value>The alignment.</value>
    Alignment Alignment { get; init; }

    /// <summary>
    /// Gets the content width of the widget, excluding any layout spacing (e.g. due to padding or alignment).
    /// </summary>
    /// <returns>The content width in pixels.</returns>
    int GetContentWidth();

    /// <summary>
    /// Gets the content height of the widget, excluding any layout spacing (e.g. due to padding or alignment).
    /// </summary>
    /// <returns>The content height in pixels.</returns>
    int GetContentHeight();

    /// <summary>
    /// Updates the widget's position and size based on the given available space.
    /// </summary>
    /// <param name="position">The top-left position of the available space.</param>
    /// <param name="availableWidth">The width of the available space in pixels.</param>
    /// <param name="availableHeight">The height of the available space in pixels.</param>
    void Update(Vector2 position, int availableWidth, int availableHeight);
}

public abstract class Widget : IWidget
{
    protected Vector2 _offset; // Offset from the position due to alignment

    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public Vector2 Position { get; protected set; } = Vector2.Zero;

    Alignment _alignment;
    public Alignment Alignment
    {
        get => _alignment;
        init
        {
            CheckMutuallyExclusive(value, Alignment.Left, Alignment.Right, Alignment.HCenter, Alignment.HStretch);
            CheckMutuallyExclusive(value, Alignment.Top, Alignment.Bottom, Alignment.VCenter, Alignment.VStretch);
            _alignment = value;
        }
    }

    public abstract int GetContentWidth();

    public abstract int GetContentHeight();

    public virtual void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        if (Alignment.HasFlag(Alignment.HStretch))
        {
            Width = availableWidth;
        }
        if (Alignment.HasFlag(Alignment.VStretch))
        {
            Height = availableHeight;
        }

        var horizontalAlignment = Alignment & (Alignment.Left | Alignment.Right | Alignment.HCenter | Alignment.HStretch);
        var verticalAlignment = Alignment & (Alignment.Top | Alignment.Bottom | Alignment.VCenter | Alignment.VStretch);
        if (horizontalAlignment == 0)
        {
            horizontalAlignment = Alignment.Left;
        }
        if (verticalAlignment == 0)
        {
            verticalAlignment = Alignment.VCenter;
        }
        Debug.Assert(horizontalAlignment.FlagCount() == 1);
        Debug.Assert(verticalAlignment.FlagCount() == 1);

        float offsetX = horizontalAlignment switch
        {
            Alignment.Left => 0f,
            Alignment.HCenter => (availableWidth - Width) / 2f,
            Alignment.Right => availableWidth - Width,
            Alignment.HStretch => 0f,
            _ => throw new ArgumentException("Invalid horizontal alignment.")
        };
        float offsetY = verticalAlignment switch
        {
            Alignment.Top => 0f,
            Alignment.VCenter => (availableHeight - Height) / 2f,
            Alignment.Bottom => availableHeight - Height,
            Alignment.VStretch => 0f,
            _ => throw new ArgumentException("Invalid vertical alignment.")
        };
        _offset = new Vector2(offsetX, offsetY);

        Position = position + _offset;
    }

    public virtual void FrameTick(IFrameTickService frameTickService)
    {
        // Do nothing by default
    }

    public virtual void Draw(IRenderer renderer)
    {
        // Do nothing by default
    }

    static void CheckMutuallyExclusive(Alignment value, params Alignment[] flags)
    {
        Alignment foundFlag1 = 0;
        foreach (var flag in flags)
        {
            if (value.HasFlag(flag))
            {
                if (foundFlag1 != 0)
                {
                    throw new ArgumentException($"Cannot have both {foundFlag1} and {flag} alignment.");
                }
                foundFlag1 = flag;
            }
        }
    }
}
