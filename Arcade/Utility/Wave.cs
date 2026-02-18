namespace Arcade.Utility;

public record Wave(float Amplitude, float Frequency, float Phase)
{
    const float TwoPi = 2 * MathF.PI;

    public float Value(float time)
    {
        return Amplitude * MathF.Sin((TwoPi * Frequency * time) + Phase);
    }
}
