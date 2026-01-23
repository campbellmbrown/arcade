using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual;

public class SpriteSheet
{
    readonly Texture2D _texture;
    readonly Dictionary<int, Rectangle> _textureSections;
    Rectangle _currentTextureRectangle;

    public Vector2 Position { get; set; }

    public SpriteSheet(Texture2D texture, Dictionary<int, Rectangle> textureSections, int initialId)
    {
        _texture = texture;
        _textureSections = textureSections;
        ChangeTextureRectangle(initialId);
    }

    public void ChangeTextureRectangle(int id)
    {
        _currentTextureRectangle = _textureSections[id];
    }

    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.Draw(_texture, Position, _currentTextureRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}
