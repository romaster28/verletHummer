using System;

public class CharacterStateMachine : ICharacterStateMachine
{
    private readonly CharacterStateFactory _stateFactory;
    private readonly ILogService _logger;
    private BaseCharacterState _state;

    public CharacterStateMachine(CharacterStateFactory stateFactory, ILogService logService)
    {
        _stateFactory = stateFactory ?? throw new ArgumentNullException(nameof(stateFactory));
        _logger = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    public bool TryGetState(out BaseCharacterState state)
    {
        state = _state;
        return state != null;
    }

    public void SetState<T>() where T : BaseCharacterState
    {
        SetState(_stateFactory.Create(typeof(T)));
    }

    public void SetState(BaseCharacterState state)
    {
        if (state == _state)
            return;
        
        _state?.Exit();
        _state = state;
        _state.Enter();
        
        _logger.Log(LogKey.Character, $"State: {state.GetType().Name}");
    }
}