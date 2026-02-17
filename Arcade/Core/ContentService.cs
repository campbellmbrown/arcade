using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Core;

public interface IContentService
{
    /// <summary>
    /// Provides access to textures.
    /// </summary>
    ITextureProvider Texture { get; }

    /// <summary>
    /// Provides access to sounds.
    /// </summary>
    ISoundProvider Sound { get; }

    /// <summary>
    /// Provides access to fonts.
    /// </summary>
    IFontProvider Font { get; }

    /// <summary>
    /// Provides access to effects.
    /// </summary>
    IEffectProvider Effect { get; }

    /// <summary>
    /// Creates visual elements.
    /// </summary>
    IContentCreator Creator { get; }
}

public class ContentService : IContentService
{
    public IFontProvider Font { get; }
    public ISoundProvider Sound { get; }
    public ITextureProvider Texture { get; }
    public IEffectProvider Effect { get; }
    public IContentCreator Creator { get; }

    public ContentService(ContentManager content, GraphicsDevice graphicsDevice)
    {
        Font = new FontProvider(content);
        Texture = new TextureProvider(content);
        Sound = new SoundProvider(content);
        Effect = new EffectProvider(graphicsDevice);
        Creator = new ContentCreator(Font);
    }
}
