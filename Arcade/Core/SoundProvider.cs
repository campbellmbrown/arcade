using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Arcade.Core;

public interface ISoundProvider
{
    /// <summary>
    /// Register and load a sound.
    /// </summary>
    /// <param name="id">The ID of the sound.</param>
    /// <param name="path">The relative path to the sound.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    void Register<T>(T id, string path) where T : Enum;

    /// <summary>
    /// Get a sound by ID.
    /// </summary>
    /// <param name="id">The ID of the sound.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    /// <returns>The sound for the given ID.</returns>
    SoundEffect Get<T>(T id) where T : Enum;

    void Play<T>(T id, float volume = 1f, float pitch = 0f, float pan = 0f) where T : Enum;
}

public class SoundProvider(ContentManager content) : ISoundProvider
{
    readonly Dictionary<Enum, SoundEffect> _sounds = [];

    public void Register<T>(T id, string path) where T : Enum
    {
        var sound = content.Load<SoundEffect>(path);
        _sounds.Add(id, sound);
    }

    public SoundEffect Get<T>(T id) where T : Enum
    {
        return _sounds[id];
    }

    public void Play<T>(T id, float volume = 1f, float pitch = 0f, float pan = 0f) where T : Enum
    {
        _sounds[id].Play(volume, pitch, pan);
    }
}
