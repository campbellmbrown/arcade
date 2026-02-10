using Arcade.Visual.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Core;

public interface IEffectProvider
{
    /// <summary>
    /// Register and load a standard effect.
    /// </summary>
    /// <remarks>
    /// Only the effects that are intended to be used should be registered.
    /// </remarks>
    /// <param name="id">The ID of the effect.</param>
    /// <param name="path">The relative path to the effect.</param>
    void RegisterStandard(StandardEffectId id, string path);

    /// <summary>
    /// Register and load an effect.
    /// </summary>
    /// <param name="id">The ID of the effect.</param>
    /// <param name="path">The relative path to the effect.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    void Register<T>(T id, string path) where T : Enum;

    /// <summary>
    /// Get a standard effect by ID.
    /// </summary>
    /// <param name="id">The ID of the effect.</param>
    /// <returns>The effect for the given ID.</returns>
    Effect GetStandard(StandardEffectId id);

    /// <summary>
    /// Get an effect by ID.
    /// </summary>
    /// <param name="id">The ID of the effect.</param>
    /// <typeparam name="T">The type of the ID.</typeparam>
    /// <returns>The effect for the given ID.</returns>
    Effect Get<T>(T id) where T : Enum;
}

public class EffectProvider(GraphicsDevice graphicsDevice) : IEffectProvider
{
    readonly Dictionary<StandardEffectId, Effect> _standardEffects = [];
    readonly Dictionary<Enum, Effect> _effects = [];

    public void RegisterStandard(StandardEffectId id, string path)
    {
        _standardEffects.Add(id, LoadEffect(path));
    }

    public void Register<T>(T id, string path) where T : Enum
    {
        _effects.Add(id, LoadEffect(path));
    }

    public Effect GetStandard(StandardEffectId id)
    {
        return _standardEffects[id];
    }

    public Effect Get<T>(T id) where T : Enum
    {
        return _effects[id];
    }

    Effect LoadEffect(string path)
    {
        return new Effect(graphicsDevice, File.ReadAllBytes(path));
    }
}
