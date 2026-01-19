using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Gui;

public class Image(Texture2D texture) : Widget
{
    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);
        renderer.SpriteBatch.Draw(texture, Position, Color.White);
    }

    protected override int IntrinsicWidth() => texture.Width;
    protected override int IntrinsicHeight() => texture.Height;
}
