using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
