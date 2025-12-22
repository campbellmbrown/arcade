using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Arcade.Core;

public interface ITextureProvider
{
    /// <summary>
    /// Register and load a texture.
    /// </summary>
    /// <param name="id">The ID of the texture.</param>
    /// <param name="path">The relative path to the texture.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    void Register<T>(T id, string path) where T : Enum;

    /// <summary>
    /// Get a texture by ID.
    /// </summary>
    /// <param name="id">The ID of the texture.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    /// <returns>The texture for the given ID.</returns>
    Texture2D Get<T>(T id) where T : Enum;
}

public class TextureProvider(ContentManager content) : ITextureProvider
{
    readonly Dictionary<Enum, Texture2D> _textures = [];


    public void Register<T>(T id, string path) where T : Enum
    {
        var texture = content.Load<Texture2D>(path);
        _textures.Add(id, texture);
    }

    public Texture2D Get<T>(T id) where T : Enum
    {
        return _textures[id];
    }
}

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

    ITextDisplay CreateTextDisplay(string text, BitmapFont font, Color color, float scale)
    {
        return new TextDisplay(text, font, color, scale);
    }
}

public interface IContentService
{
    /// <summary>
    /// Provides access to textures.
    /// </summary>
    ITextureProvider Texture { get; }

    /// <summary>
    /// Provides access to fonts.
    /// </summary>
    IFontProvider Font { get; }

    /// <summary>
    /// Creates visual elements.
    /// </summary>
    IContentCreator Creator { get; }
}

public class ContentService : IContentService
{
    public IContentCreator Creator { get; }
    public IFontProvider Font { get; }
    public ITextureProvider Texture { get; }

    public ContentService(ContentManager content)
    {
        Font = new FontProvider(content);
        Texture = new TextureProvider(content);
        Creator = new ContentCreator(Font);
    }
}
