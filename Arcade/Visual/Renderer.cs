using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Visual;

public interface IRenderer
{
    SpriteBatch SpriteBatch { get; }
    ILayerView? CurrentLayer { get; set; }
    Queue<ILight> LightQueue { get; }
    bool HasLights { get; }

    void QueueLightTexture(Texture2D texture, Vector2 position, Color color, float scale = 1f, float opacity = 1f);
    void QueueLightPoint(Vector2 position, Color color, float size = 1f, float opacity = 1f);
    void QueueLightLine(Vector2 start, Vector2 end, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightRectangle(RectangleF rectangle, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightFilledRectangle(RectangleF rectangle, Color color, float opacity = 1f);
    void QueueLightCircle(Vector2 center, float radius, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightEllipse(Vector2 center, float radiusX, float radiusY, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightEllipse(Vector2 center, Vector2 radius, Color color, float thickness = 1f, float opacity = 1f);
    void QueueLightPolygon(IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float opacity = 1f);

    void DrawBounds(RectangleF bounds);
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
    public void QueueLightTexture(Texture2D texture, Vector2 position, Color color, float scale = 1f, float opacity = 1f)
    {
        // TODO: support TextureId instead of Texture2D?
        LightQueue.Enqueue(new LightTexture(texture, position, color, scale, opacity));
    }

    public void QueueLightPoint(Vector2 position, Color color, float size = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightPoint(position, color, size, opacity));
    }

    public void QueueLightLine(Vector2 start, Vector2 end, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightLine(start, end, color, thickness, opacity));
    }

    public void QueueLightRectangle(RectangleF rectangle, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightRectangle(rectangle, color, thickness, opacity));
    }

    public void QueueLightFilledRectangle(RectangleF rectangle, Color color, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightFilledRectangle(rectangle, color, opacity));
    }

    public void QueueLightCircle(Vector2 center, float radius, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightCircle(center, radius, color, thickness, opacity));
    }

    public void QueueLightEllipse(Vector2 center, float radiusX, float radiusY, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightEllipse(center, new Vector2(radiusX, radiusY), color, thickness, opacity));
    }

    public void QueueLightEllipse(Vector2 center, Vector2 radius, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightEllipse(center, radius, color, thickness, opacity));
    }

    public void QueueLightPolygon(IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float opacity = 1f)
    {
        LightQueue.Enqueue(new LightPolygon(points, color, thickness, opacity));
    }

    public void DrawBounds(RectangleF bounds)
    {
        SpriteBatch.DrawRectangle(bounds, Color.Cyan, 1f, 1f);
    }
}
