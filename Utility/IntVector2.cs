using Microsoft.Xna.Framework;

namespace Arcade.Utility;

public struct IntVector2(int x, int y) : IEquatable<IntVector2>
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public IntVector2(int value)
        : this(value, value)
    {
    }

    public IntVector2(Vector2 value)
        : this(
            (int)Math.Round(value.X, MidpointRounding.AwayFromZero),
            (int)Math.Round(value.Y, MidpointRounding.AwayFromZero)
        )
    {
    }

    public override readonly bool Equals(object? obj) => base.Equals(obj);

    public readonly bool Equals(IntVector2 other) => (X == other.X) && (Y == other.Y);

    public override readonly int GetHashCode() => HashCode.Combine(X, Y);

    public override readonly string ToString() => $"{{X:{X} Y:{Y}}}";

    public readonly Vector2 ToVector2() => new(X, Y);

    public readonly float Length() => ToVector2().Length();

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(IntVector2 value1, IntVector2 value2) => value1.Equals(value2);
    public static bool operator !=(IntVector2 value1, IntVector2 value2) => !value1.Equals(value2);
}
