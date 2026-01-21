using Arcade.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Visual;

public interface IRenderer
{
    SpriteBatch SpriteBatch { get; }
    ILayerView? CurrentLayer { get; set; }
    Queue<LightTexture> LightQueue { get; }
    bool HasLights { get; }

    void QueueLight(Texture2D texture, Vector2 position, Color color, float scale = 1f, float opacity = 1f);

    void DrawBounds(RectangleF bounds);
}

public class LightTexture(Texture2D texture, Vector2 position, Color color, float scale, float opacity)
{
    public Color Color { get; } = color;
    public Vector2 Position { get; } = position;
    public Texture2D Texture { get; } = texture;
    public float Scale { get; } = scale;
    public float Opacity { get; } = opacity;
    public Vector2 RotationOrigin { get; } = new(texture.Width / 2f, texture.Height / 2f);
}

public class Renderer(SpriteBatch spriteBatch) : IRenderer
{
    public SpriteBatch SpriteBatch { get; } = spriteBatch;

    public Queue<LightTexture> LightQueue { get; } = new();
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

    public void DrawBounds(RectangleF bounds)
    {
        SpriteBatch.DrawRectangle(bounds, Color.Cyan, 1f, 1f);
    }
}
