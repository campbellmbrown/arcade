using System.Diagnostics;
using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Gui;

public interface IWidget : IVisual, IFrameTickable
{
    /// <summary>
    /// The resolved width of the widget, including alignment.
    /// </summary>
    /// <value>The width in pixels.</value>
    int Width { get;}

    /// <summary>
    /// The resolved width of the widget, including alignment and margins.
    /// </summary>
    /// <value>The occupied width in pixels.</value>
    int OccupiedWidth { get; }

    /// <summary>
    /// The resolved height of the widget, including alignment.
    /// </summary>
    /// <value>The height in pixels.</value>
    int Height { get; }

    /// <summary>
    /// The resolved height of the widget, including alignment and margins.
    /// </summary>
    /// <value>The occupied height in pixels.</value>
    int OccupiedHeight { get; }

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

    public Color? BorderColor { get; set; }

    public float BorderThickness { get; set; }

    /// <summary>
    /// The measured intrinsic width of the widget including margins. Can be used for calculating layout, but should
    /// not be used to identify the actual rendered width.
    /// </summary>
    /// <returns>The measured width in pixels.</returns>
    int MeasureWidth();

    /// <summary>
    /// The measured intrinsic height of the widget including margins. Can be used for calculating layout, but should
    /// not be used to identify the actual rendered height.
    /// </summary>
    /// <returns>The measured height in pixels.</returns>
    int MeasureHeight();

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
    public int OccupiedWidth => Width + MarginLeft + MarginRight;

    public int Height { get; protected set; }
    public int OccupiedHeight => Height + MarginTop + MarginBottom;

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

    public Color? BorderColor { get; set; } = null;
    public float BorderThickness { get; set; } = 1f;

    public int MeasureWidth() => IntrinsicWidth() + MarginLeft + MarginRight;

    public int MeasureHeight() => IntrinsicHeight() + MarginTop + MarginBottom;

    public virtual void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        ResolveWidth(availableWidth);
        ResolveHeight(availableHeight);
        ResolvePosition(position, availableWidth, availableHeight);
    }

    public virtual void FrameTick(IFrameTickService frameTickService)
    {
        // Do nothing by default
    }

    public virtual void Draw(IRenderer renderer)
    {
        if (BorderColor.HasValue)
        {
            renderer.SpriteBatch.DrawRectangle(new RectangleF(Position.X, Position.Y, Width, Height), BorderColor.Value, BorderThickness);
        }
        // Uncomment for debugging layout
        // renderer.SpriteBatch.DrawRectangle(new RectangleF(Position.X - MarginLeft, Position.Y - MarginTop, OccupiedWidth, OccupiedHeight), Color.Red * 0.5f, 1);
        // renderer.SpriteBatch.DrawPoint(Position, Color.Yellow, 1);
    }

    /// <summary>
    /// The intrinsic width of the widget before alignment and margins are applied.
    /// </summary>
    /// <remarks>
    /// This function should only use <see cref="MeasureWidth"/> of child widgets to calculate its value.
    /// </remarks>
    /// <returns>The intrinsic width in pixels.</returns>
    protected abstract int IntrinsicWidth();

    /// <summary>
    /// The intrinsic height of the widget before alignment and margins are applied.
    /// </summary>
    /// <remarks>
    /// This function should only use <see cref="MeasureHeight"/> of child widgets to calculate its value.
    /// </remarks>
    /// <returns>The intrinsic height in pixels.</returns>
    protected abstract int IntrinsicHeight();

    /// <summary>
    /// Resolves the width of the widget based on the available width and alignment.
    /// </summary>
    /// <param name="availableWidth">The available width in pixels.</param>
    protected virtual void ResolveWidth(int availableWidth)
    {
        Width = Alignment.HasFlag(Alignment.HStretch) ? availableWidth - MarginLeft - MarginRight : IntrinsicWidth();
    }

    /// <summary>
    /// Resolves the height of the widget based on the available height and alignment.
    /// </summary>
    /// <param name="availableHeight">The available height in pixels.</param>
    protected virtual void ResolveHeight(int availableHeight)
    {
        Height = Alignment.HasFlag(Alignment.VStretch) ? availableHeight - MarginTop - MarginBottom : IntrinsicHeight();
    }

    protected virtual void ResolvePosition(Vector2 position, int availableWidth, int availableHeight)
    {
        float offsetX = MarginLeft + HorizontalAlignment switch
        {
            Alignment.Left => 0,
            Alignment.HCenter => (availableWidth - OccupiedWidth) / 2f,
            Alignment.Right => availableWidth - OccupiedWidth,
            Alignment.HStretch => 0,
            _ => throw new ArgumentException("Invalid horizontal alignment.")
        };
        float offsetY = MarginTop + VerticalAlignment switch
        {
            Alignment.Top => 0,
            Alignment.VCenter => (availableHeight - OccupiedHeight) / 2f,
            Alignment.Bottom => availableHeight - OccupiedHeight,
            Alignment.VStretch => 0,
            _ => throw new ArgumentException("Invalid vertical alignment.")
        };
        _offset = new Vector2(offsetX, offsetY);

        Position = position + _offset;
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
