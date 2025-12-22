using Arcade.Core;

namespace Arcade.Utility;

public interface IValueGetter : ISimpleTickable
{
    object Value { get; }
}

public class ValueGetter<T>(Func<T> getter, T initial) : IValueGetter where T : notnull
{
    public object Value { get; set; } = initial;

    public void Tick()
    {
        Value = getter();
    }
}
