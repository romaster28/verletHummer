using System;

public class CharacterStateMachine : ICharacterStateMachine
{
    private readonly CharacterStatesCachedPoolProvider _characterStateProvider;
    private ICharacterState _state;

    public CharacterStateMachine(CharacterStatesCachedPoolProvider characterStateProvider)
    {
        _characterStateProvider =
            characterStateProvider ?? throw new ArgumentNullException(nameof(characterStateProvider));
    }

    public bool TryGetState(out ICharacterState state)
    {
        state = _state;
        return state != null;
    }

    public void SetState<T>() where T : ICharacterState
    {
        _state?.Exit();
        _state = _characterStateProvider.GetState<T>();
        _state.Enter();
    }
}