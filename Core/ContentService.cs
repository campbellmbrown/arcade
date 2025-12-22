using System;
using System.Collections.Generic;
using System.Linq;
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
    BitmapFont GetFont<T>(T id) where T : Enum;

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

    public BitmapFont GetFont<T>(T id) where T : Enum
    {
        return _fonts[id];
    }

    public BitmapFont Default => _fonts.First().Value;
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
    IFontProvider FontProvider { get; }
}

public class ContentService(ContentManager content) : IContentService
{
    public ITextureProvider Texture { get; } = new TextureProvider(content);
    public IFontProvider FontProvider { get; } = new FontProvider(content);
}
