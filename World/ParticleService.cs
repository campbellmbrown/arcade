using Arcade.Core;
using Arcade.Visual;

namespace Arcade.World;

public class ParticleService : IFrameTickable, IVisual
{
    readonly List<Particle> _particles = [];

    public int Count => _particles.Count;

    public void Add(Particle particle)
    {
        _particles.Add(particle);
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        for (int idx = _particles.Count - 1; idx >= 0; idx--)
        {
            _particles[idx].FrameTick(frameTickService);
            if (_particles[idx].IsDead)
            {
                _particles.RemoveAt(idx);
            }
        }
    }

    public void Draw(IRenderer renderer)
    {
        foreach (var particle in _particles)
        {
            particle.Draw(renderer);
        }
    }
}
