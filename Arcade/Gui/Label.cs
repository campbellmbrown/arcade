using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class Label : Widget
{
    readonly ITextDisplay _text;

    public Label(ITextDisplay text)
    {
        _text = text;
    }

    public void UpdateText(string text)
    {
        _text.Text = text;
    }

    public override int GetContentWidth() => (int)_text.Width;
    public override int GetContentHeight() => (int)_text.Height;

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        Width = GetContentWidth();
        Height = GetContentHeight();
        base.Update(position, availableWidth, availableHeight);
        _text.Position = Position;
    }

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);
        _text.Draw(renderer);
    }
}
