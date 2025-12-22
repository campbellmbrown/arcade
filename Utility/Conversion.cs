using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Arcade.Core.Utility;

public static class Conversion
{
    public static Vector2 PointToVector2(Point point) => new(point.X, point.Y);

    public static RectangleF Offset(RectangleF rectangle, Vector2 offset)
    {
        RectangleF offsetRectangle = rectangle;
        offsetRectangle.Offset(offset);
        return offsetRectangle;
    }

    public static RectangleF Offset(RectangleF rectangle, float xOffset, float yOffset)
    {
        RectangleF offsetRectangle = rectangle;
        offsetRectangle.Offset(xOffset, yOffset);
        return offsetRectangle;
    }
}
