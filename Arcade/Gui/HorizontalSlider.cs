using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcade.Gui;

public class HorizontalSlider(Texture2D thumbTexture, Texture2D trackTexture, float min, float max)
    : Slider(thumbTexture, trackTexture, min, max)
{
    const int DEFAULT_WIDTH = 50;

    float _thumbStopLeftX;
    float _trackY;
    Vector2 _thumbDrawPosition;

    public int? FixedWidth { get; set; } = null;

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);

        var trackEndSize = _trackTexture.Height;
        Rectangle trackLeftRect = new(0, 0, trackEndSize, trackEndSize);
        Rectangle trackRightRect = new(_trackTexture.Width - trackEndSize, 0, trackEndSize, trackEndSize);
        Rectangle trackChunkFullRect = new(trackEndSize, 0, _trackChunkLength, trackEndSize);
        Rectangle trackChunkPartialRect = new(trackEndSize, 0, _partialTrackChunkLength, trackEndSize);

        Vector2 trackDrawPosition = new(Position.X, _trackY - trackEndSize / 2f);

        // Draw the first end of the track
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackLeftRect, Color.White);
        trackDrawPosition.X += trackEndSize;
        // Draw the middle of the track in chunks. The last chunk will be partial.
        for (int i = 0; i < _numFullTrackChunks; i++)
        {
            renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackChunkFullRect, Color.White);
            trackDrawPosition.X += _trackChunkLength;
        }
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackChunkPartialRect, Color.White);
        trackDrawPosition.X += _partialTrackChunkLength;
        // Draw the last end of the track
        renderer.SpriteBatch.Draw(_trackTexture, trackDrawPosition, trackRightRect, Color.White);

        renderer.SpriteBatch.Draw(_thumbTexture, _thumbDrawPosition, Color.White);
    }

    protected override void OnDrag(Vector2 position)
    {
        SetValueFromPosition(position.X - _thumbStopLeftX);
    }

    protected override void ResolveWidth(int availableWidth)
    {
        base.ResolveWidth(availableWidth);
        NewTrackLength(Width);
    }

    protected override void ResolvePosition(Vector2 position, int availableWidth, int availableHeight)
    {
        base.ResolvePosition(position, availableWidth, availableHeight);
        _thumbStopLeftX = Position.X + _thumbTexture.Width / 2f;
        _trackY = Position.Y + (_thumbTexture.Height / 2f);
        _thumbDrawPosition = new Vector2(
            _thumbStopLeftX + GetPositionFromValue() - _thumbTexture.Width / 2f,
            _trackY - _thumbTexture.Height / 2f
        );
    }

    protected override int IntrinsicWidth() => FixedWidth ?? DEFAULT_WIDTH;

    protected override int IntrinsicHeight() => _thumbTexture.Height;
}
