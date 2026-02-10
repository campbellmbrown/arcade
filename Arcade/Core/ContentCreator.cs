using Arcade.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace Arcade.Core;

public interface IContentCreator
{
    ITextDisplay TextDisplay(string text, Color color, Enum fontId, float scale = 1f);
    ITextDisplay TextDisplay(string text, Enum fontId, float scale = 1f);
    ITextDisplay TextDisplay(string text, Color color, float scale = 1f);
    ITextDisplay TextDisplay(string text, float scale = 1f);
}

public class ContentCreator(IFontProvider fontProvider) : IContentCreator
{
    public ITextDisplay TextDisplay(string text, Color color, Enum fontId, float scale = 1f)
    {
        var font = fontProvider.Get(fontId);
        return CreateTextDisplay(text, font, color, scale);
    }

    public ITextDisplay TextDisplay(string text, Enum fontId, float scale = 1f)
    {
        return TextDisplay(text, Color.White, fontId, scale);
    }

    public ITextDisplay TextDisplay(string text, Color color, float scale = 1f)
    {
        return CreateTextDisplay(text, fontProvider.Default, color, scale);
    }

    public ITextDisplay TextDisplay(string text, float scale = 1f)
    {
        return TextDisplay(text, Color.White, scale);
    }

    static ITextDisplay CreateTextDisplay(string text, BitmapFont font, Color color, float scale)
    {
        return new TextDisplay(text, font, color, scale);
    }
}
