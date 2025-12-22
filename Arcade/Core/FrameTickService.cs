using Microsoft.Xna.Framework;

namespace Arcade.Core;

public interface IFrameTickService
{
    GameTime GameTime { set; }
    float TimeDiffSec { get; }
}

public class FrameTickService : IFrameTickService
{
    public GameTime GameTime { private get; set; } = new();
    public float TimeDiffSec => (float)GameTime.ElapsedGameTime.TotalSeconds;
}
