using Arcade.Input;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Gui;

public class Slider : Widget, IClickDraggable
{
    const int DEFAULT_WIDTH = 50;

    readonly Texture2D _trackTexture;
    readonly Texture2D _thumbTexture;

    readonly float _min;
    readonly float _max;
    readonly float _initial;
    readonly float _range;

    float _sliderTravelDistance;

    float _thumbStopLeftX;
    float _trackY;
    Vector2 _thumbDrawPosition;

    int _trackChunkLength;
    int _numFullTrackChunks;
    int _partialTrackChunkLength;

    readonly List<Action<float>> _setters = [];

    public int? FixedWidth { get; set; } = null;

    public Slider(Texture2D thumbTexture, Texture2D trackTexture, float min, float max, float initial = 0)
    {
        if (thumbTexture.Height < trackTexture.Height)
        {
            throw new ArgumentException("The thumb texture height must be greater or equal to the track texture height.");
        }
        if (min >= max)
        {
            throw new ArgumentException("The minimum value must be less than the maximum value.");
        }
        if (initial < min || initial > max)
        {
            throw new ArgumentException("The initial value must be within the range of the slider.");
        }

        _trackTexture = trackTexture;
        _thumbTexture = thumbTexture;

        _min = min;
        _max = max;
        _initial = initial;
        _range = _max - _min;
        Value = _initial;
    }

    public float Value { get; private set; }

    public RectangleF ClickArea => new(Position.X, Position.Y, Width, Height);

    public void AddSetter(Action<float> setter)
    {
        _setters.Add(setter);
    }

    public void Reset()
    {
        Value = _initial;
        foreach (var setter in _setters)
        {
            setter(Value);
        }
    }

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

    public void OnLatch()
    {
    }

    public void OnDrag(Vector2 position)
    {
        Value = MathHelper.Clamp(
            _min + _range * (position.X - _thumbStopLeftX) / _sliderTravelDistance,
            _min,
            _max
        );
        foreach (var setter in _setters)
        {
            setter(Value);
        }
    }

    public void OnRelease()
    {
    }

    protected override void ResolveWidth(int availableWidth)
    {
        base.ResolveWidth(availableWidth);
        _sliderTravelDistance = Width - _thumbTexture.Width;
        int trackLengthExcludingEnds = Width - 2 * _trackTexture.Height;
        _trackChunkLength = _trackTexture.Width - 2 * _trackTexture.Height;
        _numFullTrackChunks = trackLengthExcludingEnds / _trackChunkLength;
        _partialTrackChunkLength = trackLengthExcludingEnds % _trackChunkLength;
    }

    protected override void ResolvePosition(Vector2 position, int availableWidth, int availableHeight)
    {
        base.ResolvePosition(position, availableWidth, availableHeight);
        _thumbStopLeftX = Position.X + _thumbTexture.Width / 2f;
        _trackY = Position.Y + (_thumbTexture.Height / 2f);
        _thumbDrawPosition = new Vector2(
            _thumbStopLeftX + ((Value - _min) / _range * _sliderTravelDistance) - _thumbTexture.Width / 2f,
            _trackY - _thumbTexture.Height / 2f
        );
    }

    protected override int IntrinsicWidth() => FixedWidth ?? DEFAULT_WIDTH;

    protected override int IntrinsicHeight() => _thumbTexture.Height;
}
