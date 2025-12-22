using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Arcade.Visual;

public interface ITextDisplay : IVisual
{
    string Text { get; set; }
    Vector2 Position { get; set; }

    float Width { get; }
    float Height { get; }
    Vector2 Size { get; }
}

public class TextDisplay(string text, BitmapFont font, Color color, float scale) : ITextDisplay
{
    public string Text { get; set; } = text;
    public Vector2 Position { get; set; }

    public float Width => font.MeasureString(Text).Width * scale;
    public float Height => font.LineHeight * scale;
    public Vector2 Size => new(Width, Height);

    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawString(font, Text, Position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
    }
}
