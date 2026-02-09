using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Visual;

// TODO: move to a new file
public interface ILight : IVisual
{
}

public interface IRenderer
{
    SpriteBatch SpriteBatch { get; }
    ILayerView? CurrentLayer { get; set; }
    Queue<ILight> LightQueue { get; }
    bool HasLights { get; }

    void QueueLight(Texture2D texture, Vector2 position, Color color, float scale = 1f, float opacity = 1f);
    void QueueLightLine(Vector2 start, Vector2 end, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightPoint(Vector2 position, Color color, float size = 1f, float opacity = 1f);

    void DrawBounds(RectangleF bounds);
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

public class LightLine(Vector2 start, Vector2 end, Color color, float thickness, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawLine(start, end, color * opacity, thickness);
    }
}

public class LightPoint(Vector2 position, Color color, float size, float opacity) : ILight
{
    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawPoint(position, color * opacity, size);
    }
}

public class Renderer(SpriteBatch spriteBatch) : IRenderer
{
    public SpriteBatch SpriteBatch { get; } = spriteBatch;

    public Queue<ILight> LightQueue { get; } = new();
    public bool HasLights => LightQueue.Count > 0;

    public ILayerView? CurrentLayer { get; set; }

    /// <summary>
    /// Queue a light to be drawn.
    /// </summary>
    /// <param name="texture">The light texture.</param>
    /// <param name="position">The position (center) of the light.</param>
    /// <param name="color">The color of the light.</param>
    /// <param name="scale">The scale of the light.</param>
    /// <param name="opacity">The opacity of the light.</param>
    public void QueueLight(Texture2D texture, Vector2 position, Color color, float scale = 1f, float opacity = 1f)
    {
        // TODO: support TextureId instead of Texture2D?
        LightQueue.Enqueue(new LightTexture(texture, position, color, scale, opacity));
    }

    public void QueueLightLine(Vector2 start, Vector2 end, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightLine(start, end, color, thickness, opacity));
    }

    public void QueueLightPoint(Vector2 position, Color color, float size = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightPoint(position, color, size, opacity));
    }

    public void DrawBounds(RectangleF bounds)
    {
        SpriteBatch.DrawRectangle(bounds, Color.Cyan, 1f, 1f);
    }
}
