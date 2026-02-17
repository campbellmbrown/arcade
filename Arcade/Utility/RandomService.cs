using Microsoft.Xna.Framework;

namespace Arcade.Utility;

public interface IRandomService
{
    int NewRandomInt(int maxExclusive);
    int NewRandomInt(int minInclusive, int maxExclusive);
    double NewRandomDouble(double maxExclusive);
    double NewRandomDouble(double minInclusive, double maxExclusive);
    float NewRandomFloat(float maxExclusive);
    float NewRandomFloat(float minInclusive, float maxExclusive);
    Vector2 NewRandomVector2(float maxExclusive);
    Vector2 NewRandomVector2(float minInclusive, float maxExclusive);
    T ChooseRandom<T>(List<T> list);
    T ChooseRandom<T>(params T[] items);
}

public class RandomService : IRandomService
{
    readonly Random _random = new();

    public int NewRandomInt(int maxExclusive) => _random.Next(maxExclusive);

    public int NewRandomInt(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

    public double NewRandomDouble(double maxExclusive) => _random.NextDouble() * maxExclusive;

    public double NewRandomDouble(double minInclusive, double maxExclusive) =>
        _random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;

    public float NewRandomFloat(float maxExclusive) => (float)NewRandomDouble(maxExclusive);

    public float NewRandomFloat(float minInclusive, float maxExclusive) => (float)NewRandomDouble(minInclusive, maxExclusive);

    public Vector2 NewRandomVector2(float maxExclusive) => new(NewRandomFloat(maxExclusive), NewRandomFloat(maxExclusive));
    public Vector2 NewRandomVector2(float minInclusive, float maxExclusive) => new(NewRandomFloat(minInclusive, maxExclusive), NewRandomFloat(minInclusive, maxExclusive));

    public T ChooseRandom<T>(List<T> list) => list[_random.Next(list.Count)];
    public T ChooseRandom<T>(params T[] items) => items[_random.Next(items.Length)];
}
