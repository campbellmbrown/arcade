using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Gui;

public class VerticalSlider(Texture2D thumbTexture, Texture2D trackTexture, float min, float max)
    : Slider(thumbTexture, trackTexture, min, max)
{
    const int DEFAULT_HEIGHT = 50;

    float _thumbStopTopY;
    float _trackX;
    Vector2 _thumbDrawPosition;

    public int? FixedHeight { get; set; } = null;

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);

        var trackEndSize = _trackTexture.Height;
        Rectangle trackLeftRect = new(0, 0, trackEndSize, trackEndSize);
        Rectangle trackRightRect = new(_trackTexture.Width - trackEndSize, 0, trackEndSize, trackEndSize);
        Rectangle trackChunkFullRect = new(trackEndSize, 0, _trackChunkLength, trackEndSize);
        Rectangle trackChunkPartialRect = new(trackEndSize, 0, _partialTrackChunkLength, trackEndSize);

        Vector2 trackDrawPosition = new(_trackX + trackEndSize / 2f, Position.Y);

        // Draw the first end of the track
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackLeftRect, Color.White, MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        trackDrawPosition.Y += trackEndSize;
        // Draw the middle of the track in chunks. The last chunk will be partial.
        for (int i = 0; i < _numFullTrackChunks; i++)
        {
            renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackChunkFullRect, Color.White, MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            trackDrawPosition.Y += _trackChunkLength;
        }
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackChunkPartialRect, Color.White, MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        trackDrawPosition.Y += _partialTrackChunkLength;
        // Draw the last end of the track
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackRightRect, Color.White, MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        renderer.SpriteBatch.Draw(_thumbTexture, _thumbDrawPosition, null, Color.White, -MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    protected override void OnDrag(Vector2 position)
    {
        SetValueFromPosition(position.Y - _thumbStopTopY);
    }

    protected override void ResolveHeight(int availableHeight)
    {
        base.ResolveHeight(availableHeight);
        NewTrackLength(Height);
    }

    protected override void ResolvePosition(Vector2 position, int availableWidth, int availableHeight)
    {
        base.ResolvePosition(position, availableWidth, availableHeight);
        _thumbStopTopY = Position.Y + _thumbTexture.Width / 2f;
        _trackX = Position.X + (_thumbTexture.Height / 2f);
        _thumbDrawPosition = new Vector2(
            _trackX - _thumbTexture.Height / 2f,
            _thumbStopTopY + GetPositionFromValue() + _thumbTexture.Width / 2f
        );
    }

    protected override int IntrinsicWidth() => _thumbTexture.Height;

    protected override int IntrinsicHeight() => FixedHeight ?? DEFAULT_HEIGHT;
}
