using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Visual.Effects;

public interface IGameEffect
{
    Effect Effect { get; }

    void PrepareEffect();
}
