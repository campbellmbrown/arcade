using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Gui;

public class ImageButton(Texture2D texture) : ButtonBase
{
    Vector2 _centerOffset = new(texture.Width / 2f, texture.Height / 2f);

    // Enough to shrink by 1 pixel either way
    readonly float _latchScale = (texture.Width - 2) / (float)texture.Width;

    public Color HoverBackgroundColor { get; set; } = Color.White * 0.3f;

    protected override int IntrinsicHeight() => texture.Height;

    protected override int IntrinsicWidth() => texture.Width;

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);

        if (IsHovering)
        {
            renderer.SpriteBatch.FillRectangle(InteractionArea, HoverBackgroundColor);
        }

        float scale = IsLatched ? _latchScale : 1.0f;
        renderer.SpriteBatch.Draw(texture, Position + _centerOffset, null, Color.White, 0f, _centerOffset, scale, SpriteEffects.None, 0f);
    }
}
