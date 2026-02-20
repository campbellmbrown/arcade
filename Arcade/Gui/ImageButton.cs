using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Gui;

public class ImageButton(Texture2D texture) : ButtonBase
{
    Vector2 _centerOffset = new(texture.Width / 2f, texture.Height / 2f);

    // Enough to shrink by 1 pixel either way
    float _latchScale = (texture.Width - 2) / (float)texture.Width;

    public void ChangeTexture(Texture2D newTexture)
    {
        texture = newTexture;
        _centerOffset = new Vector2(texture.Width / 2f, texture.Height / 2f);
        _latchScale = (texture.Width - 2) / (float)texture.Width;
    }

    public override void Draw(IRenderer renderer)
    {
        float scale = InputEvent.IsLatched ? _latchScale : 1.0f;
        renderer.SpriteBatch.Draw(texture, Position + _centerOffset, null, Color.White, 0f, _centerOffset, scale, SpriteEffects.None, 0f);
        base.Draw(renderer);
    }

    protected override int IntrinsicHeight() => texture.Height;

    protected override int IntrinsicWidth() => texture.Width;
}
