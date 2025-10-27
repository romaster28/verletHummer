public interface ICharacterStateMachine
{
    bool TryGetState(out ICharacterState state);
    void SetState<T>() where T : ICharacterState;
}