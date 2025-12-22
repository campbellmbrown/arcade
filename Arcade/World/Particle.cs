using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.World;

public class Particle(Texture2D texture, Vector2 position) : IFrameTickable, IVisual
{
    public Texture2D Texture { get; set; } = texture;
    public Vector2 RotationOrigin { get; } = new Vector2(texture.Width / 2f, texture.Height / 2f);

    public Vector2 Position { get; set; } = position;
    public float Scale { get; set; } = 1f;
    public float Rotation { get; set; } = 0f;

    public Vector2 Direction { get; set; } = Vector2.Zero;

    public float Speed { get; set; } = 0f;
    public float AngularVelocity { get; set; } = 0f;
    public float ScaleIncreaseRate { get; set; } = 0f;

    public float Deceleration { get; set; } = 0f;

    public float Opacity { get; set; } = 1f;
    public float FadeRate { get; set; } = 0.25f;
    public float FadeAfter { get; set; } = 2f;

    public float LifeTime { get; set; } = 0f;
    public bool IsDead { get; private set; } = false;

    public void FrameTick(IFrameTickService frameTickService)
    {
        Speed = Math.Max(0, Speed - Deceleration * frameTickService.TimeDiffSec);
        Vector2 Velocity = Direction * Speed;
        Position += Velocity * frameTickService.TimeDiffSec;
        Scale += ScaleIncreaseRate * frameTickService.TimeDiffSec;
        Rotation += AngularVelocity * frameTickService.TimeDiffSec;
        LifeTime += frameTickService.TimeDiffSec;

        if (LifeTime >= FadeAfter)
        {
            Opacity -= FadeRate * frameTickService.TimeDiffSec;
        }

        if (Opacity <= 0)
        {
            Opacity = 0;
            IsDead = true;
        }
    }

    public void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.Draw(Texture, Position, null, Color.White * Opacity, Rotation, RotationOrigin, Scale, SpriteEffects.None, 0.9f);
    }
}
