using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;

namespace Arcade.Core;

public interface IFontProvider
{
    /// <summary>
    /// Register and load a font.
    /// </summary>
    /// <param name="id">The ID of the font.</param>
    /// <param name="path">The relative path to the font.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    void Register<T>(T id, string path) where T : Enum;

    /// <summary>
    /// Get a font by ID.
    /// </summary>
    /// <param name="id">The ID of the font.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    /// <returns>The font for the given ID.</returns>
    BitmapFont Get<T>(T id) where T : Enum;

    BitmapFont Default { get; }
}

public class FontProvider(ContentManager content) : IFontProvider
{
    readonly Dictionary<Enum, BitmapFont> _fonts = [];

    public void Register<T>(T id, string path) where T : Enum
    {
        var font = content.Load<BitmapFont>(path);
        _fonts.Add(id, font);
    }

    public BitmapFont Get<T>(T id) where T : Enum
    {
        return _fonts[id];
    }

    public BitmapFont Default => _fonts.First().Value;
}
