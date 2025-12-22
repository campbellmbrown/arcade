namespace Arcade.Core;

/// <summary>
/// Generic class for managing state transitions.
/// </summary>
/// <typeparam name="TStateId">The type of the state identifier.</typeparam>
public class StateSwitch<TStateId> where TStateId : struct, Enum
{
    readonly Queue<TStateId> _requestedStateIds = new();

    public void RequestNewState(TStateId newStateId)
    {
        _requestedStateIds.Enqueue(newStateId);
    }

    public TStateId? GetRequestedState()
    {
        if (_requestedStateIds.Count > 0)
        {
            var stateId = _requestedStateIds.Dequeue();
            _requestedStateIds.Clear();
            return stateId;
        }
        return null;
    }
}
