using Arcade.Core;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Visual;

/// <summary>
/// Represents a segment of a path.
/// </summary>
public record PathSegment(Vector2 Start, Vector2 End, Color Color) : IVisual
{
    /// <summary>
    /// The lifetime of this segment.
    /// </summary>
    public float LifeTime { get; set; } = 0f;

    /// <summary>
    /// The opacity of this segment.
    /// </summary>
    public float Opacity { get; set; } = 1f;

    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawLine(Start, End, Color * Opacity, 1f, 1f);
    }
}

/// <summary>
/// A path trace that fades over time.
/// </summary>
/// <param name="initial">The initial point of the path.</param>
/// <param name="fadeAfter">The time after which the path starts to fade.</param>
/// <param name="fadeRate">The rate at which the path fades.</param>
/// <param name="color">The color of the path.</param>
public class PathTrace(Vector2 initial, float fadeAfter, float fadeRate, Color color) : IFrameTickable, IVisual
{
    readonly List<PathSegment> _segments = [];
    Vector2 _lastPoint = initial;

    /// <summary>
    /// Adds a point to the path.
    /// </summary>
    /// <param name="point">The point to add.</param>
    public void Add(Vector2 point)
    {
        _segments.Add(new PathSegment(_lastPoint, point, color));
        _lastPoint = point;
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        for (int idx = _segments.Count - 1; idx >= 0; idx--)
        {
            var segment = _segments[idx];
            segment.LifeTime += frameTickService.TimeDiffSec;
            if (segment.LifeTime > fadeAfter)
            {
                segment.Opacity -= fadeRate * frameTickService.TimeDiffSec;
                if (segment.Opacity <= 0)
                {
                    _segments.RemoveAt(idx);
                }
            }
        }
    }

    public void Draw(IRenderer renderer)
    {
        foreach (var segment in _segments)
        {
            segment.Draw(renderer);
        }
    }
}
