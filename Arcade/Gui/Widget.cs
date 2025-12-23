using System.Diagnostics;
using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Gui;

public interface IWidget : IVisual, IFrameTickable
{
    /// <summary>
    /// The width of the widget in pixels, including alignment (e.g. stretch) but excluding margins.
    /// </summary>
    /// <value>The width in pixels.</value>
    int Width { get;}

    /// <summary>
    /// The width of the widget including margins in pixels.
    /// </summary>
    /// <value>The outer width in pixels.</value>
    int OuterWidth { get; }

    /// <summary>
    /// The height of the widget in pixels, including alignment (e.g. stretch) but excluding margins.
    /// </summary>
    /// <value>The height in pixels.</value>
    int Height { get; }

    /// <summary>
    /// The width of the widget including margins in pixels.
    /// </summary>
    /// <value>The outer height in pixels.</value>
    int OuterHeight { get; }

    /// <summary>
    /// The position of the widget in the view.
    /// </summary>
    /// <value>The position.</value>
    Vector2 Position { get; }

    /// <summary>
    /// The alignment of the widget within its allocated space.
    /// </summary>
    /// <value>The alignment.</value>
    Alignment Alignment { get; set; }

    /// <summary>
    /// The horizontal alignment of the widget within its allocated space derived from <see cref="Alignment"/>.
    /// </summary>
    /// <value>The horizontal alignment.</value>
    Alignment HorizontalAlignment { get; }

    /// <summary>
    /// The vertical alignment of the widget within its allocated space derived from <see cref="Alignment"/>.
    /// </summary>
    /// <value>The vertical alignment.</value>
    Alignment VerticalAlignment { get; }

    /// <summary>
    /// The top margin of the widget in pixels.
    /// </summary>
    /// <value>The top margin in pixels.</value>
    public int MarginTop { get; set; }

    /// <summary>
    /// The bottom margin of the widget in pixels.
    /// </summary>
    /// <value>The bottom margin in pixels.</value>
    public int MarginBottom { get; set; }

    /// <summary>
    /// The left margin of the widget in pixels.
    /// </summary>
    /// <value>The left margin in pixels.</value>
    public int MarginLeft { get; set; }

    /// <summary>
    /// The right margin of the widget in pixels.
    /// </summary>
    /// <value>The right margin in pixels.</value>
    public int MarginRight { get; set; }

    /// <summary>
    /// Sets all margins (top, bottom, left, right) to the given value.
    /// </summary>
    /// <value>The margin in pixels.</value>
    public int MarginAll { set; }

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
    public int OuterWidth => Width + MarginLeft + MarginRight;

    public int Height { get; protected set; }
    public int OuterHeight => Height + MarginTop + MarginBottom;

    public Vector2 Position { get; protected set; } = Vector2.Zero;
    public int MarginTop { get; set; } = 0;
    public int MarginBottom { get; set; } = 0;
    public int MarginLeft { get; set; } = 0;
    public int MarginRight { get; set; } = 0;

    public int MarginAll
    {
        set
        {
            MarginTop = value;
            MarginBottom = value;
            MarginLeft = value;
            MarginRight = value;
        }
    }

    Alignment _alignment = Alignment.Left | Alignment.VCenter;
    public Alignment Alignment
    {
        get => _alignment;
        set
        {
            CheckMutuallyExclusive(value, Alignment.Left, Alignment.Right, Alignment.HCenter, Alignment.HStretch);
            CheckMutuallyExclusive(value, Alignment.Top, Alignment.Bottom, Alignment.VCenter, Alignment.VStretch);
            _alignment = value;

            if (HorizontalAlignment == 0)
            {
                _alignment |= Alignment.Left;
            }
            if (VerticalAlignment == 0)
            {
                _alignment |= Alignment.VCenter;
            }
            Debug.Assert(_alignment.FlagCount() == 2);
        }
    }

    public Alignment HorizontalAlignment => Alignment & (Alignment.Left | Alignment.Right | Alignment.HCenter | Alignment.HStretch);
    public Alignment VerticalAlignment => Alignment & (Alignment.Top | Alignment.Bottom | Alignment.VCenter | Alignment.VStretch);

    public abstract int GetContentWidth();

    public abstract int GetContentHeight();

    public virtual void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        if (Alignment.HasFlag(Alignment.HStretch))
        {
            Width = availableWidth - MarginLeft - MarginRight;
        }
        if (Alignment.HasFlag(Alignment.VStretch))
        {
            Height = availableHeight - MarginTop - MarginBottom;
        }

        float offsetX = HorizontalAlignment switch
        {
            Alignment.Left => MarginLeft,
            Alignment.HCenter => MarginLeft + (availableWidth - OuterWidth) / 2f,
            Alignment.Right => availableWidth - OuterWidth,
            Alignment.HStretch => MarginLeft,
            _ => throw new ArgumentException("Invalid horizontal alignment.")
        };
        float offsetY = VerticalAlignment switch
        {
            Alignment.Top => MarginTop,
            Alignment.VCenter => MarginTop + (availableHeight - OuterHeight) / 2f,
            Alignment.Bottom => availableHeight - OuterHeight,
            Alignment.VStretch => MarginTop,
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
        renderer.SpriteBatch.DrawRectangle(new RectangleF(Position.X - MarginLeft, Position.Y - MarginTop, OuterWidth, OuterHeight), Color.Red * 0.5f, 1);
        renderer.SpriteBatch.DrawRectangle(new RectangleF(Position.X, Position.Y, Width, Height), Color.Blue * 0.5f, 1);
        renderer.SpriteBatch.DrawPoint(Position, Color.Yellow, 1);
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
