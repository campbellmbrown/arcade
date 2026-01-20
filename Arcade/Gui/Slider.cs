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

    public float Min { get; set; }
    public float Max { get; set; }
    public float Range => Max - Min;

    float _value;
    public float Value
    {
        get => _value;
        set
        {
            if ((value < Min) || (value > Max))
            {
                throw new ArgumentOutOfRangeException(nameof(Value), $"Value must be between {Min} and {Max}.");
            }
            _value = value;
            ValueChanged?.Invoke(_value);
        }
    }

    public RectangleF InteractionArea => new(Position.X, Position.Y, Width, Height);
    public ClickDragEvent InputEvent { get; } = new();

    public event Action<float>? ValueChanged;

    protected Slider(Texture2D thumbTexture, Texture2D trackTexture, float min, float max)
    {
        // TODO: add a way to register default thumb and track textures.
        if (thumbTexture.Height < trackTexture.Height)
        {
            throw new ArgumentException("The thumb texture height must be greater or equal to the track texture height.");
        }
        if (min >= max)
        {
            throw new ArgumentException("The minimum value must be less than the maximum value.");
        }

        _trackTexture = trackTexture;
        _thumbTexture = thumbTexture;
        _trackChunkLength = _trackTexture.Width - (2 * _trackTexture.Height);

        Min = min;
        Max = max;
        _value = Min;

        InputEvent.Dragged += OnDrag;
    }

    public virtual void SetValueFromPosition(float distanceFromStart)
    {
        var portion = distanceFromStart / _sliderTravelDistance;
        Value = MathHelper.Clamp(Min + Range * portion, Min, Max);
    }

    protected abstract void OnDrag(Vector2 position);

    protected float GetPositionFromValue() => (Value - Min) / Range * _sliderTravelDistance;

    protected void NewTrackLength(int trackLength)
    {
        _sliderTravelDistance = trackLength - _thumbTexture.Width;
        int trackLengthExcludingEnds = trackLength - (2 * _trackTexture.Height);
        _numFullTrackChunks = trackLengthExcludingEnds / _trackChunkLength;
        _partialTrackChunkLength = trackLengthExcludingEnds % _trackChunkLength;
    }
}
