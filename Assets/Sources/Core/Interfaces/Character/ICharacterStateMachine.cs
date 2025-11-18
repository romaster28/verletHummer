public interface ICharacterStateMachine
{
    bool TryGetState(out BaseCharacterState state);
    void SetState<T>() where T : BaseCharacterState;
    void SetState(BaseCharacterState state);
}