using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Visual;

public interface ILight : IVisual
{
}

public class LightTexture(Texture2D texture, Vector2 position, Color color, float scale, float opacity) : ILight
{
    public Color Color { get; } = color;
    public Vector2 Position { get; } = position;
    public Texture2D Texture { get; } = texture;
    public float Scale { get; } = scale;
    public float Opacity { get; } = opacity;
    public Vector2 RotationOrigin { get; } = new(texture.Width / 2f, texture.Height / 2f);

    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.Draw(
            Texture,
            Position,
            null,
            Color * Opacity,
            0f,
            RotationOrigin,
            Scale,
            SpriteEffects.None,
            0f
        );
    }
}

public class LightPoint(Vector2 position, Color color, float size, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawPoint(position, color * opacity, size);
    }
}

public class LightLine(Vector2 start, Vector2 end, Color color, float thickness, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawLine(start, end, color * opacity, thickness);
    }
}

public class LightRectangle(RectangleF rectangle, Color color, float thickness, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawRectangle(rectangle, color * opacity, thickness);
    }
}

public class LightFilledRectangle(RectangleF rectangle, Color color, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.FillRectangle(rectangle, color * opacity);
    }
}

public class LightCircle(Vector2 center, float radius, Color color, float thickness, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawCircle(center, radius, 32, color * opacity, thickness);
    }
}

public class LightEllipse(Vector2 center, Vector2 radius, Color color, float thickness, float opacity, int segments = 32) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawEllipse(center, radius, segments, color * opacity, thickness);
    }
}

public class LightPolygon(IReadOnlyList<Vector2> points, Color color, float thickness, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawPolygon(Vector2.Zero, points, color * opacity, thickness);
    }
}
