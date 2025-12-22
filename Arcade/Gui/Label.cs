using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public class Label : Widget
{
    readonly ITextDisplay _text;

    public Label(ITextDisplay text)
    {
        _text = text;
        Width = (int)_text.Width;
        Height = (int)_text.Height;
    }

    public void UpdateText(string text)
    {
        _text.Text = text;
        Width = (int)_text.Width;
        Height = (int)_text.Height;
    }

    public override int GetContentWidth() => (int)_text.Width;
    public override int GetContentHeight() => (int)_text.Height;

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        _text.Position = Position;
        base.Update(position, availableWidth, availableHeight);
    }

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);
        _text.Draw(renderer);
    }
}
