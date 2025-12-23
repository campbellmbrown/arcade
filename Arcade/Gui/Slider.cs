using Arcade.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Gui;

public abstract class Slider : Widget, IClickDraggable
{
    readonly protected Texture2D _trackTexture;
    readonly protected Texture2D _thumbTexture;
    readonly protected int _trackChunkLength;

    protected float _sliderTravelDistance;

    protected int _numFullTrackChunks;
    protected int _partialTrackChunkLength;

    readonly float _min;
    readonly float _max;
    readonly float _initial;
    readonly float _range;

    readonly List<Action<float>> _setters = [];

    public float Value { get; protected set; }

    public RectangleF ClickArea => new(Position.X, Position.Y, Width, Height);

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
        _trackChunkLength = _trackTexture.Width - (2 * _trackTexture.Height);

        _min = min;
        _max = max;
        _initial = initial;
        _range = _max - _min;
        Value = _initial;
    }

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

    public void OnLatch()
    {
    }

    public abstract void OnDrag(Vector2 position);

    public virtual void SetValueFromPosition(float distanceFromStart)
    {
        var portion = distanceFromStart / _sliderTravelDistance;
        Value = MathHelper.Clamp(_min + _range * portion, _min, _max);
        foreach (var setter in _setters)
        {
            setter(Value);
        }
    }

    public void OnRelease()
    {
    }

    protected float GetPositionFromValue() => (Value - _min) / _range * _sliderTravelDistance;

    protected void NewTrackLength(int trackLength)
    {
        _sliderTravelDistance = trackLength - _thumbTexture.Width;
        int trackLengthExcludingEnds = trackLength - (2 * _trackTexture.Height);
        _numFullTrackChunks = trackLengthExcludingEnds / _trackChunkLength;
        _partialTrackChunkLength = trackLengthExcludingEnds % _trackChunkLength;
    }
}
