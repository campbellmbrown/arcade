using Arcade.Visual;

namespace Arcade.Core;

public class GameStateService<TStateId>(
    ISpriteBatchService spriteBatchService,
    TStateId defaultStateId
) : IVisual, IFrameTickable
    where TStateId : struct, Enum
{
    readonly StateSwitch<TStateId> _stateSwitch = new();
    readonly Dictionary<TStateId, IGameState> _states = [];

    TStateId _stateId = defaultStateId;
    bool _isSetupComplete = false;

    public void RegisterState(TStateId stateId, IGameState state)
    {
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
                throw new Exception($"State {value} is not registered");
            }
        }
        _isSetupComplete = true;
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        if (!_isSetupComplete)
        {
            throw new Exception("Setup is not complete");
        }

        _states[_stateId].FrameTick(frameTickService);

        var requestedState = _stateSwitch.GetRequestedState();
        if (requestedState.HasValue)
        {
            _stateId = requestedState.Value.StateId;
            _states[_stateId].Enter(requestedState.Value.Parameter);
        }
    }

    public void Draw(IRenderer renderer)
    {
        if (!_isSetupComplete)
        {
            throw new Exception("Setup is not complete");
        }

        spriteBatchService.Start(DrawType.Gui);
        _states[_stateId].Draw(renderer);
        spriteBatchService.Finish();
    }
}
