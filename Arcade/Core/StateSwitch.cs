namespace Arcade.Core;

public readonly record struct StateTransition<TStateId>(TStateId StateId, object? Parameter = null) where TStateId : struct, Enum;

/// <summary>
/// Generic class for managing state transitions.
/// </summary>
/// <typeparam name="TStateId">The type of the state identifier.</typeparam>
public class StateSwitch<TStateId> where TStateId : struct, Enum
{
    StateTransition<TStateId>? _requested;

    /// <summary>
    /// Requests a transition to a new state.
    /// </summary>
    /// <param name="newStateId">The identifier of the new state.</param>
    /// <param name="parameter">An optional parameter for the new state.</param>
    public void RequestNewState(TStateId newStateId, object? parameter = null)
    {
        _requested = new StateTransition<TStateId>(newStateId, parameter);
    }

    public StateTransition<TStateId>? GetRequestedState()
    {
        var r = _requested;
        _requested = null;
        return r;
    }
}
