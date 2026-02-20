namespace Arcade.Utility;

public record Wave
{
    const float TwoPi = 2 * MathF.PI;

    public Wave(float amplitude, float frequency, float phase)
    {
        Amplitude = amplitude;
        Frequency = frequency;
        Phase = phase;
    }

    public float Amplitude { get; set; }
    public float Frequency { get; set; }
    public float Phase { get; set; }

    public float Value(float time)
    {
        return Amplitude * MathF.Sin((TwoPi * Frequency * time) + Phase);
    }
}
