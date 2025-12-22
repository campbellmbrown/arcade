using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public record PartialTexture(Texture2D Texture, Rectangle SourceRectangle)
{
    public PartialTexture(Texture2D texture, int x, int y, int width, int height)
        : this(texture, new Rectangle(x, y, width, height))
    {
    }

    public int Width => SourceRectangle.Width;
    public int Height => SourceRectangle.Height;

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        spriteBatch.Draw(Texture, position, SourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float scale, Color color, float layerDepth)
    {
        spriteBatch.Draw(Texture, position, SourceRectangle, color, 0f, origin, scale, SpriteEffects.None, layerDepth);
    }
}
