using Arcade.Input;
using Arcade.Visual;

namespace Arcade.Core;

public class GameStateService<TStateId>(
    IDrawService drawService,
    IInputServiceGeneric _input,
    TStateId defaultStateId
) : IVisual, IFrameTickable
    where TStateId : struct, Enum
{
    readonly StateSwitch<TStateId> _stateSwitch = new();
    readonly Dictionary<TStateId, IGameState<TStateId>> _states = [];


    TStateId _stateId = defaultStateId;
    bool _isSetupComplete = false;

    public void RegisterState(TStateId stateId, IGameState<TStateId> state)
    {
        state.StateSwitch = _stateSwitch;
        _states.Add(stateId, state);
    }

    public void CompleteSetup()
    {
        if (_isSetupComplete)
        {
            throw new InvalidOperationException("Attempted to complete setup when already completed.");
        }
        foreach (var value in Enum.GetValues<TStateId>())
        {
            if (!_states.ContainsKey(value))
            {
                throw new InvalidOperationException($"State {value} has not been registered.");
            }
        }
        _isSetupComplete = true;
        _states[_stateId].Enter();
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        if (!_isSetupComplete)
        {
            throw new InvalidOperationException("Setup is not complete");
        }

        _states[_stateId].FrameTick(frameTickService);

        var requestedState = _stateSwitch.GetRequestedState();
        if (requestedState.HasValue)
        {
            _input.TearDown();
            _stateId = requestedState.Value.StateId;
            _states[_stateId].Enter(requestedState.Value.Parameter);
        }
    }

    public void Draw(IRenderer renderer)
    {
        if (!_isSetupComplete)
        {
            throw new InvalidOperationException("Setup is not complete");
        }

        drawService.Start(DrawType.Gui);
        _states[_stateId].Draw(renderer);
        drawService.Finish();
    }
}
